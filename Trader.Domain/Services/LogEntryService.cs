using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using DynamicData;
using Trader.Domain.Infrastucture;

namespace Trader.Domain.Services
{
    public class LogEntryService : IDisposable, ILogEntryService
    {
        private readonly ISourceCache<LogEntry, long> _source = new SourceCache<LogEntry, long>(l => l.Key);
        private readonly IDisposable _disposer;
        
        public LogEntryService(ILogger logger)
        {

            var loader = ReactiveLogAppender.LogEntryObservable
                            .Subscribe(_source.AddOrUpdate);


            //limit size of cache to prevent too many items being created
            var sizeLimiter = _source.LimitSizeTo(10000).Subscribe();

            // alternatively could expire by time
            //var timeExpirer = _source.ExpireAfter(le => TimeSpan.FromSeconds(le.Level == LogLevel.Debug ? 5 : 60), TimeSpan.FromSeconds(5), TaskPoolScheduler.Default)
            //                            .Subscribe(removed => logger.Debug("{0} log items have been automatically removed", removed.Count()));

            _disposer = new CompositeDisposable(sizeLimiter, _source, loader);
            logger.Info("Log cache has been constructed");
        }


        public IObservableCache<LogEntry, long> Items
        {
            get { return _source.AsObservableCache(); }
        }

        public void Add(LogEntry item)
        {
            _source.AddOrUpdate(item);
        }

        public void Remove(IEnumerable<LogEntry> items)
        {
            _source.Remove(items);
        }

        public void Remove(IEnumerable<long> keys)
        {
            _source.Remove(keys);
        }

        public void Dispose()
        {
            _disposer.Dispose();
        }
    }
}