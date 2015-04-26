using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class StaticData : IStaticData
    {
        private readonly CurrencyPair[] _currencyPairs =
        {
            new CurrencyPair("GBP/USD",1.6M,4,5M) ,
            new CurrencyPair("EUR/USD",1.23904M,4,3M),
            new CurrencyPair("EUR/GBP",0.7913M,4,2M),
            new CurrencyPair("NZD/CAD",0.8855M,4,0.5M)  ,
            new CurrencyPair("HKD/USD",0.128908M,6,0.01M) ,
            new CurrencyPair("NOK/SEK",1.10M,3,2M) ,
            new CurrencyPair("XAU/GBP",768.399M,3,0.5M) ,
            new CurrencyPair("USD/JPY",118.81M,2,0.1M),
        };

        private readonly string[] _customers = new[] { "Bank of America", "Bank of Europe", "Bank of England", "BNP Paribas", "Deutsche Bank", "Societe Generale" };


        public string[] Customers
        {
            get { return _customers; }
        }

        public CurrencyPair[] CurrencyPairs
        {
            get { return _currencyPairs; }
        }
    }
}