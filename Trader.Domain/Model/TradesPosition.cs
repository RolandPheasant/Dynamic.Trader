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


        public decimal Position
        {
            get { return _position; }
        }

        public decimal Buy
        {
            get { return _buy; }
        }

        public decimal Sell
        {
            get { return _sell; }
        }

        public string CountText
        {
            get { return "Order".Pluralise(_count); }
        }

        public bool Negative
        {
            get { return _position < 0; }
        }

        public int Count
        {
            get { return _count; }
        }
    }
}