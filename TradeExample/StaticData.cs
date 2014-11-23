namespace TradeExample
{
    public class StaticData : IStaticData
    {
        public class CurrencyPair
        {
            private readonly string _code;
            private readonly decimal _startingPrice;

            public CurrencyPair(string code, decimal startingPrice)
            {
                _code = code;
                _startingPrice = startingPrice;
            }

            public string Code
            {
                get { return _code; }
            }

            public decimal InitialPrice
            {
                get { return _startingPrice; }
            }


        }

        private readonly CurrencyPair[] _currencyPairs =
        {
            new CurrencyPair("GBP/USD",1.6M) ,
            new CurrencyPair("EUR/USD",1.23904M),
            new CurrencyPair("EUR/GBP",0.791339614M),
            new CurrencyPair("NZD/CAD",0.885535855M)  
        };

        private readonly string[] _customers = new[] { "Bank of America", "Bank of Europe", "Bank of England","BNP Paribas" };


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