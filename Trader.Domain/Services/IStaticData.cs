namespace Trader.Domain.Services
{
    public interface IStaticData
    {
        string[] Customers { get; }
        StaticData.CurrencyPair[] CurrencyPairs { get; }
    }
}