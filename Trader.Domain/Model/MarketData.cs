using System;

namespace Trader.Domain.Model;

public class MarketData : IEquatable<MarketData>
{
    public MarketData(string instrument, decimal bid, decimal offer)
    {
        Instrument = instrument;
        Bid = bid;
        Offer = offer;
    }

    public string Instrument { get; }
    public decimal Bid { get; }
    public decimal Offer { get; }

    #region Equality

    public bool Equals(MarketData other)
    {
        return string.Equals(Instrument, other.Instrument) && Bid == other.Bid && Offer == other.Offer;
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
            int hashCode = (Instrument != null ? Instrument.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Bid.GetHashCode();
            hashCode = (hashCode * 397) ^ Offer.GetHashCode();
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
        return $"{Instrument}, {Bid}/{Offer}";
    }
}