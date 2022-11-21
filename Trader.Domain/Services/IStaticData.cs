using Trader.Domain.Model;

namespace Trader.Domain.Services;

public interface IStaticData
{
    string[] Customers { get; }
    CurrencyPair[] CurrencyPairs { get; }
}