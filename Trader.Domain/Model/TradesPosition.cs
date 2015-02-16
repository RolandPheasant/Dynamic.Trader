using System;

namespace Trader.Domain.Model
{
    public class TradesPosition
    {
        private readonly decimal _buy;
        private readonly decimal _sell;
        private readonly int _count;


        public TradesPosition(decimal buy, decimal sell, int count)
        {
            _buy = buy;
            _sell = sell;
            _count = count;
        }

        public decimal Position
        {
            get { return _buy - _sell; }
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

        public int Count
        {
            get { return _count; }
        }
    }
}