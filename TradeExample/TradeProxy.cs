using System;
using TradeExample.Infrastucture;

namespace TradeExample
{
    public class TradeProxy:AbstractNotifyPropertyChanged, IDisposable, IEquatable<TradeProxy>
    {
        private readonly Trade _trade;
        private readonly IDisposable _cleanUp;

        public TradeProxy(Trade trade)
        {
            _trade = trade;


            //market price changed is a obserble on the trade object
            _cleanUp = trade.MarketPriceChanged
                .Subscribe(_ => OnPropertyChanged("MarketPrice"));

        }

        public decimal MarketPrice
        {
            get { return _trade.MarketPrice; }
        }

        // delegating menbers below

        #region Delegating Members
        
        public long Id
        {
            get { return _trade.Id; }
        }

        public string CurrencyPair
        {
            get { return _trade.CurrencyPair; }
        }

        public string Customer
        {
            get { return _trade.Customer; }
        }

        public TradeStatus Status
        {
            get { return _trade.Status; }
        }

        public DateTime Timestamp
        {
            get { return _trade.Timestamp; }
        }

        public decimal TradePrice
        {
            get { return _trade.TradePrice; }
        }


        public decimal PercentFromMarket
        {
            get { return _trade.PercentFromMarket; }
        }

        #endregion

        #region Equaility Members

        public bool Equals(TradeProxy other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_trade, other._trade);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TradeProxy) obj);
        }

        public override int GetHashCode()
        {
            return (_trade != null ? _trade.GetHashCode() : 0);
        }

        public static bool operator ==(TradeProxy left, TradeProxy right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TradeProxy left, TradeProxy right)
        {
            return !Equals(left, right);
        }

        #endregion

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}