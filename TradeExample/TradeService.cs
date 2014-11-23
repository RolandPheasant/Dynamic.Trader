using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;

namespace TradeExample
{
    public class TradeService : ITradeService, IDisposable
    {
        private readonly ISourceCache<Trade, long> _tradesSource;
        private readonly IObservableCache<Trade, long> _tradesCache;
        private readonly IDisposable _cleanup;

        public TradeService()
        {
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
            var generator = new TradeGenerator();
            var random = new Random();

            //initally create 1000 trades then create up to 10 every second
            var tradeGenerator = Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(_ => random.Next(1, 10))
                .StartWith(1000)
                .Do(number => Console.WriteLine("Adding {0} trades", number))
                .Select(generator.Generate)
                .Subscribe(_tradesSource.AddOrUpdate);

            //close up to 10 trades every  second
            var tradeCloser = Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(_ => random.Next(1, 10))
                .Do(number => Console.WriteLine("Closing {0} trades", number))
                .Select(number=> _tradesSource.Items
                                    .Where(trade=>trade.Status== TradeStatus.Live)
                                    .OrderBy(t=>Guid.NewGuid()).Take(number))
                .Subscribe(trades =>
                           {
                               var toClose = trades
                                   .Select(trade => new Trade(trade.Id,trade.Customer,trade.CurrencyPair,TradeStatus.Closed,trade.Price));
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