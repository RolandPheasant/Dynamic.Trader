using System;

namespace Trader.Domain.Services
{
    public interface IMarketPriceService
    {
        IObservable<decimal> ObservePrice(string currencyPair);
    }
}