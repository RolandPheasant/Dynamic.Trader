using System;

namespace Trader.Domain.Model
{
    public struct MarketData : IEquatable<MarketData>
    {
        private readonly string _instrument;
        private readonly decimal _bid;
        private readonly decimal _offer;

        public MarketData(string instrument, decimal bid, decimal offer)
        {
            _instrument = instrument;
            _bid = bid;
            _offer = offer;
        }

        public string Instrument
        {
            get { return _instrument; }
        }

        public decimal Bid
        {
            get { return _bid; }
        }

        public decimal Offer
        {
            get { return _offer; }
        }



        #region Equality

        public bool Equals(MarketData other)
        {
            return string.Equals(_instrument, other._instrument) && _bid == other._bid && _offer == other._offer;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MarketData && Equals((MarketData)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (_instrument != null ? _instrument.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _bid.GetHashCode();
                hashCode = (hashCode * 397) ^ _offer.GetHashCode();
                return hashCode;
            }
        }


        public static MarketData operator +(MarketData left, decimal pipsValue)
        {
            var bid = left.Bid + pipsValue;
            var offer = left.Offer + pipsValue;
            return new MarketData(left.Instrument, bid, offer);
        }

        public static MarketData operator -(MarketData left, decimal pipsValue)
        {
            var bid = left.Bid - pipsValue;
            var offer = left.Offer - pipsValue;
            return new MarketData(left.Instrument, bid, offer);
        }

        public static bool operator >=(MarketData left, MarketData right)
        {
            return left.Bid >= right.Bid;
        }

        public static bool operator <=(MarketData left, MarketData right)
        {
            return left.Bid <= right.Bid;
        }

        public static bool operator >(MarketData left, MarketData right)
        {
            return left.Bid > right.Bid;
        }

        public static bool operator <(MarketData left, MarketData right)
        {
            return left.Bid < right.Bid;
        }

        public static bool operator ==(MarketData left, MarketData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MarketData left, MarketData right)
        {
            return !left.Equals(right);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}, {1}/{2}", _instrument, _bid, _offer);
        }
    }
}