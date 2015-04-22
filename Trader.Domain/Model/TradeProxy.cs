using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
using DynamicData.Operators;
using Trader.Domain.Infrastucture;

namespace Trader.Domain.Model
{


    public class SortParameterData : AbstractNotifyPropertyChanged
    {
        private SortContainer _selectedItem;
        private readonly IList<SortContainer> _sortItems = new List<SortContainer>
                   {
                       new SortContainer("Customer, Currency Pair", SortExpressionComparer<TradeProxy>
                           .Ascending(l => l.Customer)
                           .ThenByAscending(p => p.CurrencyPair)
                           .ThenByAscending(p => p.Id)),

                       new SortContainer("Currency Pair, Amount", SortExpressionComparer<TradeProxy>
                           .Ascending(l => l.CurrencyPair)
                           .ThenByDescending(p => p.Amount)
                           .ThenByAscending(p => p.Id)),

                           
                       new SortContainer("Recently Changed", SortExpressionComparer<TradeProxy>
                           .Descending(l => l.Timestamp)
                           .ThenByAscending(p => p.Customer)
                           .ThenByAscending(p => p.Id)),
                   };


        public SortParameterData()
        {
            SelectedItem = _sortItems[2];
        }

        public SortContainer SelectedItem
        {
            get { return _selectedItem; }
            private set { SetAndRaise(ref _selectedItem, value); }
        }

        public IEnumerable<SortContainer> SortItems
        {
            get { return _sortItems; }
        }
    }

    public sealed class SortContainer : IEquatable<SortContainer>
    {
        private readonly IComparer<TradeProxy> _comparer;
        private readonly string _description;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SortContainer(string description, IComparer<TradeProxy> comparer)
        {
            _description = description;
            _comparer = comparer;
        }

        public IComparer<TradeProxy> Comparer
        {
            get { return _comparer; }
        }

        public string Description
        {
            get { return _description; }
        }

        #region Equality members

        public bool Equals(SortContainer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_description, other._description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SortContainer)obj);
        }

        public override int GetHashCode()
        {
            return (_description != null ? _description.GetHashCode() : 0);
        }

        public static bool operator ==(SortContainer left, SortContainer right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SortContainer left, SortContainer right)
        {
            return !Equals(left, right);
        }

        #endregion
    }

    public class TradeProxy:AbstractNotifyPropertyChanged, IDisposable, IEquatable<TradeProxy>
    {
        private readonly Trade _trade;
        private readonly IDisposable _cleanUp;
        private bool _recent;
        private readonly long _id;
        private decimal _marketPrice;
        private decimal _pcFromMarketPrice;

        public TradeProxy(Trade trade)
        {
            _id = trade.Id;
            _trade = trade;

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
                    MarketPrice = trade.MarketPrice;
                    PercentFromMarket = trade.PercentFromMarket;
                });
        
            _cleanUp = Disposable.Create(() =>
                                         {
                                             recentIndicator.Dispose();
                                             priceRefresher.Dispose();
                                         });

        }

        public bool Recent
        {
            get { return _recent; }
            set { SetAndRaise(ref _recent, value); }
        }

        public decimal MarketPrice
        {
            get { return _marketPrice; }
            set { SetAndRaise(ref _marketPrice, value); }
        }

        public decimal PercentFromMarket
        {
            get { return _pcFromMarketPrice; }
            set { SetAndRaise(ref _pcFromMarketPrice, value); }
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

        public decimal Amount
        {
            get { return _trade.Amount; }
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