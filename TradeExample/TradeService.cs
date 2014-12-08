using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using TradeExample.Infrastucture;

namespace TradeExample
{
    public class TradeService : ITradeService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly TradeGenerator _tradeGenerator;
        private readonly ISourceCache<Trade, long> _tradesSource;
        private readonly IObservableCache<Trade, long> _tradesCache;
        private readonly IDisposable _cleanup;

        public TradeService(ILogger logger,TradeGenerator tradeGenerator)
        {
            _logger = logger;
            _tradeGenerator = tradeGenerator;

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
            _tradesSource.AddOrUpdate(_tradeGenerator.Generate(1000, true));
            
            // create up to 10  periodically
            var tradeGenerator = Observable.Interval(TimeSpan.FromSeconds(updatePeriod))
                .Select(_ => random.Next(1, 10))
                .Do(number => _logger.Info("Adding {0} trades", number))
                .Select(number=>_tradeGenerator.Generate(number))
                .Subscribe(_tradesSource.AddOrUpdate);

            //close up to 10 trades periodically
            var tradeCloser = Observable.Interval(TimeSpan.FromSeconds(updatePeriod))
                .Select(_ => random.Next(1, 10))
                .Do(number => _logger.Info("Closing {0} trades", number))
                .Select(number=> _tradesSource.Items
                                    .Where(trade=>trade.Status== TradeStatus.Live)
                                    .OrderBy(t=>Guid.NewGuid()).Take(number))
                .Subscribe(trades =>
                           {
                               var toClose = trades
                                   .Select(trade => new Trade(trade.Id,trade.Customer,trade.CurrencyPair,TradeStatus.Closed,trade.MarketPrice));
                               _tradesSource.AddOrUpdate(toClose);                  
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