using System;
using System.Reactive.Linq;

namespace TradeExample
{
    public static class MarketPriceService

    {
        public static IObservable<decimal> ObservePrice(string currencyPair)
        {
            return Observable.Empty<decimal>();
        }
    }
}