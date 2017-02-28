using System;

namespace Trader.Domain.Model
{
    public class TradesPosition
    {
        private readonly int _count;
        
        public TradesPosition(decimal buy, decimal sell, int count)
        {
            Buy = buy;
            Sell = sell;
            _count = count;
            Position = Buy - Sell;
        }
        
        public decimal Position { get; }
        public decimal Buy { get; }
        public decimal Sell { get; }
        public string CountText => "Order".Pluralise(_count);
    }
}