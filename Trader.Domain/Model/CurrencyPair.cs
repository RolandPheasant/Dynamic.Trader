using System;

namespace Trader.Domain.Model
{
    public class CurrencyPair
    {
        public string Code { get; }
        public decimal InitialPrice { get; }
        public int DecimalPlaces { get; }
        public decimal TickFrequency { get; }
        public decimal PipSize { get; }
        public int DefaultSpread { get; }

        public CurrencyPair(string code, decimal startingPrice, int decimalPlaces, decimal tickFrequency, int defaultSpread = 8)
        {
            Code = code;
            InitialPrice = startingPrice;
            DecimalPlaces = decimalPlaces;
            TickFrequency = tickFrequency;
            DefaultSpread = defaultSpread;
            PipSize = (decimal)Math.Pow(10, -decimalPlaces);
        }



        #region Equality

        protected bool Equals(CurrencyPair other)
        {
            return string.Equals(Code, other.Code);
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
            return (Code != null ? Code.GetHashCode() : 0);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Code: {0}, DecimalPlaces: {1}", Code, DecimalPlaces);
        }
    }
}