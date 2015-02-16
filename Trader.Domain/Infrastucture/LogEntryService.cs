using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using DynamicData;

namespace Trader.Domain.Infrastucture
{
    public class LogEntryService:   IDisposable, ILogEntryService
    {
        private readonly ISourceCache<LogEntry,long> _source = new SourceCache<LogEntry, long>(l=>l.Key); 
        private readonly ILogger _logger;
        private readonly IDisposable _disposer;


        public LogEntryService(ILogger logger)
        {
            _logger = logger;

            var scheduler = new EventLoopScheduler();

            ////expire old items
            //var timeExpirer = _source.ExpireAfter(le => TimeSpan.FromSeconds(le.Level == LogLevel.Debug ? 5 : 60),TimeSpan.FromSeconds(5),  TaskPoolScheduler.Default)
            //                            .Subscribe(removed =>
            //                            {
            //                                logger.Debug("{0} log items have been automatically removed",removed.Count());
            //                            });

          //  var expirer = _source.ExpireFromSource(50).Subscribe();




            _disposer = Disposable.Create(() =>
                                              {
                                               ///  timeExpirer.Dispose();
                                                   // expirer.Dispose();
                                                    scheduler.Dispose();
                                                    //loader.Dispose();
                                                    _source.Dispose();
                                              });

            _logger.Info("Log cache has been constructed");
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