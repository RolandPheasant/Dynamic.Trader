namespace TradeExample.Services
{
    public interface IStaticData
    {
        string[] Customers { get; }
        StaticData.CurrencyPair[] CurrencyPairs { get; }

    }
}