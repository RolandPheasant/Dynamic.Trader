using System;

namespace Trader.Domain.Model
{
    public class TradesPosition
    {
        private readonly decimal _buy;
        private readonly decimal _sell;
        private readonly int _count;
        private readonly decimal _position;


        public TradesPosition(decimal buy, decimal sell, int count)
        {
            _buy = buy;
            _sell = sell;
            _count = count;
            _position = _buy - _sell;
        }


        public decimal Position => _position;

        public decimal Buy => _buy;

        public decimal Sell => _sell;

        public string CountText => "Order".Pluralise(_count);

        public bool Negative => _position < 0;

        public int Count => _count;
    }
}