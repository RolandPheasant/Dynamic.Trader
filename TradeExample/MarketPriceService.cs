using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using DynamicData.Kernel;

namespace TradeExample
{
    public interface IMarketPriceService
    {
        IObservable<decimal> ObservePrice(string currencyPair);
    }

    public  class MarketPriceService : IMarketPriceService
    {
        private readonly Dictionary<string, IObservable<decimal>> _prices =  new Dictionary<string, IObservable<decimal>>();
        
        public MarketPriceService(IStaticData staticData)
        {
            foreach (var item in staticData.CurrencyPairs)
            {
                _prices[item.Code] = GenerateStream(item.InitialPrice);
            }
        }

        private IObservable<decimal> GenerateStream(decimal initalPrice)
        {
            return Observable.Create<decimal>(observer =>
                                              {

                                                  var currentPrice = initalPrice;
                                                  observer.OnNext(currentPrice);

                                                  var random = new Random();
                                                  var period = random.Next(250, 1500);
                                                  
                                                  //for a given period, move prices by up to 5 pips
                                                  return Observable.Interval(TimeSpan.FromMilliseconds(period))
                                                      .Select(_ => (decimal)random.Next(1, 5) / (decimal)1000)
                                                      .Subscribe(pips =>
                                                                 {
                                                                     currentPrice = random.NextDouble() > 0.5
                                                                         ? currentPrice + pips
                                                                         : currentPrice - pips;

                                                                     observer.OnNext(currentPrice);

                                                                 });
                                              });
        }
        
        public  IObservable<decimal> ObservePrice(string currencyPair)
        {
            return _prices.Lookup(currencyPair)
                .ValueOrThrow(() => new Exception(currencyPair + " is an unknown currency pair"));
        }
    }
}