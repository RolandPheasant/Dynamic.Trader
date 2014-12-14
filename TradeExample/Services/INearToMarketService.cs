using System;
using DynamicData;
using TradeExample.Annotations;
using TradeExample.Model;

namespace TradeExample.Services
{
    public interface INearToMarketService
    {
        IObservable<IChangeSet<Trade, long>> Query([NotNull] Func<uint> percentFromMarket);
    }
}