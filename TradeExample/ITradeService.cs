using DynamicData;

namespace TradeExample
{
    public interface ITradeService
    {
        IObservableCache<Trade, long> Trades { get; }
    }
}