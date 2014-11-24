using System;
using DynamicData;
using TradeExample.Annotations;

namespace TradeExample
{
    public interface INearToMarketService
    {
        IObservable<IChangeSet<Trade, long>> Query([NotNull] Func<uint> percentFromMarket);
    }
}