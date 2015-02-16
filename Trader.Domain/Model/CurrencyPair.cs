using System;

namespace Trader.Domain.Model
{
    public class CurrencyPair
    {
        private readonly string _code;
        private readonly decimal _startingPrice;
        private readonly int _decimalPlaces;
        private readonly decimal _tickFrequency;
        private readonly int _defaultSpread;
        private readonly decimal _pipSize;

        public CurrencyPair(string code, decimal startingPrice, int decimalPlaces, decimal tickFrequency, int defaultSpread = 8)
        {
            _code = code;
            _startingPrice = startingPrice;
            _decimalPlaces = decimalPlaces;
            _tickFrequency = tickFrequency;
            _defaultSpread = defaultSpread;
            _pipSize = (decimal)Math.Pow(10, -decimalPlaces);
        }

        public string Code
        {
            get { return _code; }
        }

        public decimal InitialPrice
        {
            get { return _startingPrice; }
        }

        public int DecimalPlaces
        {
            get { return _decimalPlaces; }
        }

        public decimal TickFrequency
        {
            get { return _tickFrequency; }
        }

        public decimal PipSize
        {
            get { return _pipSize; }
        }

        public int DefaultSpread
        {
            get { return _defaultSpread; }
        }

        #region Equality

        protected bool Equals(CurrencyPair other)
        {
            return string.Equals(_code, other._code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CurrencyPair)obj);
        }

        public override int GetHashCode()
        {
            return (_code != null ? _code.GetHashCode() : 0);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Code: {0}, DecimalPlaces: {1}", _code, _decimalPlaces);
        }
    }
}