using System;
using DynamicData;
using TradeExample.Annotations;
using Trader.Domain.Model;

namespace Trader.Domain.Services;

public interface INearToMarketService
{
    IObservable<IChangeSet<Trade, long>> Query([NotNull] Func<decimal> percentFromMarket);
}