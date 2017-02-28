using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class StaticData : IStaticData
    {
        public string[] Customers { get; } = new[]
        {
            "Bank of Andorra",
            "Bank of Europe",
            "Bank of England",
            "BCCI",
            "Abbey National",
            "Fx Shop",
            "Midland Bank",
            "National Bank of Alaska",
            "Northern Rock"
        };

        public CurrencyPair[] CurrencyPairs { get; } = {
            new CurrencyPair("GBP/USD",1.6M,4,5M) ,
            new CurrencyPair("EUR/USD",1.23904M,4,3M),
            new CurrencyPair("EUR/GBP",0.7913M,4,2M),
            new CurrencyPair("NZD/CAD",0.8855M,4,0.5M)  ,
            new CurrencyPair("HKD/USD",0.128908M,6,0.01M) ,
            new CurrencyPair("NOK/SEK",1.10M,3,2M) ,
            new CurrencyPair("XAU/GBP",768.399M,3,0.5M) ,
            new CurrencyPair("USD/JPY",118.81M,2,0.1M),
        };
    }
}