using System;
using System.Collections.Generic;
using System.Linq;

namespace TradeExample
{
    public class TradeGenerator
    {
        private readonly string[] _banks= new[] { "Bank of America", "Bank of Europe", "Bank of England" };
        private readonly string[] _currencyPairs= new[] { "GBP/USD", "EUR/USD", "EUR/GBP", "NZD/CAD" };
        // private readonly TradeStatus[] _statuslist = new[] { TradeStatus.Closed, TradeStatus.Live };

        private readonly Random _random= new Random();

        public IEnumerable<Trade> Generate(int numberToGenerate)
        {
            Func<Trade> newTrade = () =>
                                   {
                                       var id = Guid.NewGuid().GetHashCode();
                                       var bank = _banks[_random.Next(0, _banks.Length - 1)];
                                       var pair = _currencyPairs[_random.Next(0, _currencyPairs.Length - 1)];
                                       //var status = _statuslist[_random.Next(0, _statuslist.Length - 1)];
                                       return new Trade(id, bank, pair, TradeStatus.Live);
                                   };
            return Enumerable.Range(1, numberToGenerate).Select(_ => newTrade());
        }
    }
}