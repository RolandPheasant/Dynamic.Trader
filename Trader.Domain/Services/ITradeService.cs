using DynamicData;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public interface ITradeService
    {
        IObservableCache<Trade, long> Trades { get; }
    }
}