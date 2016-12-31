using System;

namespace Trader.Domain.Model
{
    public class TradesPosition
    {
        public decimal Buy { get; }
        public decimal Sell { get; }
        public decimal Position { get; }

        public int Count { get; }
        public string CountText => "Order".Pluralise(Count);

        public bool Negative => Position < 0;

        public TradesPosition(decimal buy, decimal sell, int count)
        {
            Buy = buy;
            Sell = sell;
            Count = count;
            Position = Buy - Sell;
        }

    }
}