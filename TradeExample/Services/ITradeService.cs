using DynamicData;
using TradeExample.Model;

namespace TradeExample.Services
{
    public interface ITradeService
    {
        IObservableCache<Trade, long> Trades { get; }
    }
}