using System;
using Trader.Domain.Model;

namespace Trader.Domain.Services;

public interface IMarketDataService
{
    IObservable<MarketData> Watch(string currencyPair);
}