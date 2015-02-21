using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using DynamicData;
using DynamicData.Kernel;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class TradeService : ITradeService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly TradeGenerator _tradeGenerator;
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly ISourceCache<Trade, long> _tradesSource;
        private readonly IObservableCache<Trade, long> _all;
        private readonly IObservableCache<Trade, long> _live;
        private readonly IDisposable _cleanup;

        public TradeService(ILogger logger,TradeGenerator tradeGenerator, ISchedulerProvider schedulerProvider)
        {
            _logger = logger;
            _tradeGenerator = tradeGenerator;
            _schedulerProvider = schedulerProvider;

            //construct a cache specifying that the primary key is Trade.Id
            _tradesSource = new SourceCache<Trade, long>(trade => trade.Id);

            //call AsObservableCache() to hide the update methods as we are exposing the cache
            _all = _tradesSource.AsObservableCache();

            //create a child cache 
            _live = _tradesSource.Connect(trade => trade.Status == TradeStatus.Live).AsObservableCache();

            //code to emulate an external trade provider
            var tradeLoader = GenerateTradesAndMaintainCache();

            //log changes
            var loggerWriter = LogChanges();
            
            _cleanup = new CompositeDisposable(_all, _tradesSource, tradeLoader, loggerWriter);
        }
        
        private IDisposable GenerateTradesAndMaintainCache()
        {
            //bit of code to generate trades
            var random = new Random();

            //initally load some trades 
            _tradesSource.AddOrUpdate(_tradeGenerator.Generate(5000, true));

            Func<TimeSpan> randomInterval = () => {
                                                        var ms = random.Next(150, 5000);
                                                        return TimeSpan.FromMilliseconds(ms);
                                                    };

            // create a random number of trades at a random interval
            var tradeGenerator = _schedulerProvider.TaskPool
                            .ScheduleRecurringAction(randomInterval, () =>
                            {
                                var number = random.Next(1,7);
                                var trades = _tradeGenerator.Generate(number);
                                _tradesSource.AddOrUpdate(trades);
                            });
           
            // close a random number of trades at a random interval
            var tradeCloser = _schedulerProvider.TaskPool
                .ScheduleRecurringAction(randomInterval, () =>
                {
                    var number = random.Next(1, 8);
                    _tradesSource.BatchUpdate(updater =>
                                              {
                                                  var trades = updater.Items
                                                    .Where(trade => trade.Status == TradeStatus.Live)
                                                    .OrderBy(t => Guid.NewGuid()).Take(number).ToArray();

                                                  var toClose = trades
                                                      .Select(trade => new Trade(trade, TradeStatus.Closed));

                                                  _tradesSource.AddOrUpdate(toClose);
                                              });
                });

            return new CompositeDisposable(tradeGenerator, tradeCloser);

        }

        private IDisposable LogChanges()
        {
            //todo: Move this to a log writing service
            const string messageTemplate = "{0} {1} {2} ({4}). Status = {3}";
            return _all.Connect().SkipInitial()
                                    .Transform(trade => string.Format(messageTemplate,
                                        trade.BuyOrSell,
                                        trade.Amount,
                                        trade.CurrencyPair,
                                        trade.Status,
                                        trade.Customer))
                                    .Subscribe(changes => changes.ForEach(change => _logger.Info(change.Current)));

        }

        public IObservableCache<Trade, long> All
        {
            get { return _all; }
        }

        public IObservableCache<Trade, long> Live
        {
            get { return _live; }
        }

        public void Dispose()
        {
            _cleanup.Dispose();
        }
    }
}