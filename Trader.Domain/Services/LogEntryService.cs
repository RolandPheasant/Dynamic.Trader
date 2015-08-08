using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Trader.Domain.Infrastucture;

namespace Trader.Domain.Services
{
    public class LogEntryService : IDisposable, ILogEntryService
    {
        private readonly ILogger _logger;
        private readonly ISourceList<LogEntry> _source = new SourceList<LogEntry>();
        private readonly IObservableList<LogEntry> _readonly; 
        private readonly IDisposable _disposer;
        private readonly object _locker = new object();
        
        public LogEntryService(ILogger logger)
        {
            _logger = logger;
            _readonly = _source.AsObservableList();

            var loader = ReactiveLogAppender.LogEntryObservable
                            .Buffer(TimeSpan.FromMilliseconds(250))
                            .Synchronize(_locker)
                            .Subscribe(_source.AddRange);
            
            //limit size of cache to prevent too many items being created
            var sizeLimiter = _source.LimitSizeTo(10000).Subscribe();

            // alternatively could expire by time
            //var timeExpirer = _source.ExpireAfter(le => TimeSpan.FromSeconds(le.Level == LogLevel.Debug ? 5 : 60), TimeSpan.FromSeconds(5), TaskPoolScheduler.Default)
            //                            .Subscribe(removed => logger.Debug("{0} log items have been automatically removed", removed.Count()));

            _disposer = new CompositeDisposable(sizeLimiter, _source, loader);
            logger.Info("Log cache has been constructed");
        }


        public IObservableList<LogEntry> Items
        {
            get { return _readonly; }
        }

        public void Remove(IEnumerable<LogEntry> items)
        {
            lock (_locker)
            {
                var itemsToRemove = items as LogEntry[] ?? items.ToArray();
                _logger.Info("Removing {0} log entry items",itemsToRemove.Count());
                _source.RemoveMany(itemsToRemove);
                _logger.Info("Removed {0} log entry items", itemsToRemove.Count());
            }

        }

        public void Dispose()
        {
            _disposer.Dispose();
        }
    }
}