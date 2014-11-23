using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using DynamicData.Kernel;
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

            _cleanUp = trade.MarketPriceChanged
                .Subscribe(_ => OnPropertyChanged("MarketPrice"));

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

        public decimal MarketPrice
        {
            get { return _trade.MarketPrice; }
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

    public class TradesViewer :AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IDisposable _cleanUp;
        private readonly IObservableCollection<TradeProxy> _data = new ObservableCollectionExtended<TradeProxy>();

        private readonly FilterController<Trade> _filter = new FilterController<Trade>();
        private string _searchText;

        public TradesViewer(ILogger logger,ITradeService tradeService)
        {
            _logger = logger;

            var filterApplier = this.ObserveChanges()
                                .Sample(TimeSpan.FromMilliseconds(250))
                                .Subscribe(_ => ApplyFilter());

            ApplyFilter();

            var loader = tradeService.Trades
                .Connect(trade => trade.Status == TradeStatus.Live)
                .Filter(_filter)
                .Transform(trade => new TradeProxy(trade))
                .Sort(SortExpressionComparer<TradeProxy>.Descending(t => t.Timestamp))
                .ObserveOnDispatcher()
                .Bind(_data)
                .Subscribe();

            _cleanUp = new CompositeDisposable(loader, _filter, filterApplier);
        }

        private void ApplyFilter()
        {
            _logger.Info("Applying filter");
            if (string.IsNullOrEmpty(SearchText))
            {
                _filter.ChangeToIncludeAll();
            }
            else
            {
                _filter.Change(t => t.CurrencyPair.Contains(SearchText,StringComparison.OrdinalIgnoreCase) ||
                                    t.Customer.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }
            _logger.Info("Applied filter");
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public IObservableCollection<TradeProxy> Data
        {
            get { return _data; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}