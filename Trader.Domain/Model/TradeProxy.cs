using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Trader.Domain.Infrastucture;

namespace Trader.Domain.Model
{
    public class TradeProxy:AbstractNotifyPropertyChanged, IDisposable, IEquatable<TradeProxy>
    {
        private readonly Trade _trade;
        private readonly IDisposable _cleanUp;
        private bool _recent;
        private readonly long _id;

        public TradeProxy(Trade trade)
        {
            _id = trade.Id;
            _trade = trade;

           // var isRecent = trade.Timestamp.Subtract(DateTime.Now).TotalSeconds < 2;

            var isRecent = DateTime.Now.Subtract(trade.Timestamp).TotalSeconds < 2;
            var recentIndicator= Disposable.Empty;
                         
            if (isRecent)
            {
                Recent = true;
                recentIndicator = Observable.Timer(TimeSpan.FromSeconds(2))
                    .Subscribe(_ => Recent = false);
            }


            //market price changed is an observable on the trade object
            var priceRefresher = trade.MarketPriceChanged
                .Subscribe(_ =>
                           {
                               OnPropertyChanged("MarketPrice");
                               OnPropertyChanged("PercentFromMarket");
                           });
        
            _cleanUp = Disposable.Create(() =>
                                         {
                                             recentIndicator.Dispose();
                                             priceRefresher.Dispose();
                                         });

        }

        public decimal MarketPrice
        {
            get { return _trade.MarketPrice; }
        }

        public bool Recent
        {
            get { return _recent; }
            set
            {
                if (_recent==value) return;
                _recent = value;
                OnPropertyChanged();
            }
        }

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
            return _id == other._id;
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
            return _id.GetHashCode();
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