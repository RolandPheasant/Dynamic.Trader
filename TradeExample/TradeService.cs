using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Kernel;
using TradeExample.Infrastucture;

namespace TradeExample
{
    public class TradeService : ITradeService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly TradeGenerator _tradeGenerator;
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly ISourceCache<Trade, long> _tradesSource;
        private readonly IObservableCache<Trade, long> _tradesCache;
        private readonly IDisposable _cleanup;

        public TradeService(ILogger logger,TradeGenerator tradeGenerator, ISchedulerProvider schedulerProvider)
        {
            _logger = logger;
            _tradeGenerator = tradeGenerator;
            _schedulerProvider = schedulerProvider;

            //construct a cache specifying that the unique key is Trade.Id
            _tradesSource = new SourceCache<Trade, long>(trade => trade.Id);

            //call AsObservableCache() to hide the update methods as we are exposing the cache
            _tradesCache = _tradesSource.AsObservableCache();

            //code to emulate an external trade provider
            var tradeLoader = GenerateTradesAndMaintainCache();

            _cleanup = new CompositeDisposable(_tradesCache, _tradesSource, tradeLoader);
        }


        private IDisposable GenerateTradesAndMaintainCache()
        {
            //bit of code to generate trades
            var random = new Random();

            const int updatePeriod = 10;

            //initally load 1000 trades 
            _tradesSource.AddOrUpdate(_tradeGenerator.Generate(10000, true));

            Func<TimeSpan> randomInterval = () =>
                                            {
                                                var ms = random.Next(500, 5000);
                                                return TimeSpan.FromMilliseconds(ms);
                                            };

            // create a random number of trades at a random interval
            var tradeGenerator = _schedulerProvider.TaskPool
                            .ScheduleRecurringAction(randomInterval, () =>
                            {
                                var number = random.Next(1, 5);
                                _logger.Info("Adding {0} trades", number);
                                var trades = _tradeGenerator.Generate(number);
                                _tradesSource.AddOrUpdate(trades);
                            });
           
            // close a random number of trades at a random interval
            var tradeCloser = _schedulerProvider.TaskPool
                .ScheduleRecurringAction(randomInterval, () =>
                {
                    
                    
                    
                    var number = random.Next(1, 5);
                    _logger.Info("Closing {0} trades", number);
                    
                    
                    _tradesSource.BatchUpdate(updater =>
                                              {
                                                  var trades = _tradesSource.Items
                                                    .Where(trade => trade.Status == TradeStatus.Live)
                                                    .OrderBy(t => Guid.NewGuid()).Take(number).ToArray();

                                                  var toClose = trades
                                                      .Select(trade => new Trade(trade.Id, trade.Customer, trade.CurrencyPair, TradeStatus.Closed, trade.MarketPrice));

                                                  _tradesSource.AddOrUpdate(toClose);
                                              });

                });

            return new CompositeDisposable(tradeGenerator, tradeCloser);

        }


        public IObservableCache<Trade, long> Trades
        {
            get { return _tradesCache; }
        }


        public void Dispose()
        {
            _cleanup.Dispose();
        }
    }
}