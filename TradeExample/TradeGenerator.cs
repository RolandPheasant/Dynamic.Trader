using System;
using System.Collections.Generic;
using System.Linq;

namespace TradeExample
{
    public class TradeGenerator
    {
        private readonly Random _random= new Random();
        private readonly IStaticData _staticData;
     
        public TradeGenerator(IStaticData staticData)
        {
            _staticData = staticData;
        }

        public IEnumerable<Trade> Generate(int numberToGenerate)
        {
            Func<Trade> newTrade = () =>
                                   {
                                       var id = Guid.NewGuid().GetHashCode();
                                       var bank = _staticData.Customers[_random.Next(0, _staticData.Customers.Length - 1)];
                                       var pair = _staticData.CurrencyPairs[_random.Next(0, _staticData.CurrencyPairs.Length - 1)];
                                       return new Trade(id, bank, pair.Code, TradeStatus.Live, GererateRandomPrice(pair.InitialPrice));
                                   };
            return Enumerable.Range(1, numberToGenerate).Select(_ => newTrade());
        }


        private decimal GererateRandomPrice(decimal initalPrice)
        {
            //generate percent price 1-10% away from the market
            var pcFromMarket = _random.Next(1, 1000)/(decimal) 10000;

            //logic says I should enter 2 as the upper but alas no
            var positive = _random.Next(1, 3) == 1;

            return positive
                ? initalPrice + (pcFromMarket*initalPrice)
                : initalPrice - (pcFromMarket*initalPrice);
        }
    }
}