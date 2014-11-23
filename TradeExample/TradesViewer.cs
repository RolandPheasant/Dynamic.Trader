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