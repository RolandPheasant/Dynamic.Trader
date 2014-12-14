using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using DynamicData.Kernel;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class NearToMarketViewer : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IDisposable _cleanUp;
        private readonly IObservableCollection<TradeProxy> _data = new ObservableCollectionExtended<TradeProxy>();
        private readonly FilterController<Trade> _filter = new FilterController<Trade>();
        private string _searchText;
        private uint _nearToMarketPercent=5;

        public NearToMarketViewer(ILogger logger, INearToMarketService nearToMarketService)
        {
            _logger = logger;

            var filterApplier = this.ObserveChanges()
                .Sample(TimeSpan.FromMilliseconds(250))
                .Subscribe(_ => ApplyFilter());


             
            ApplyFilter();

            var loader = nearToMarketService.Query(() => NearToMarketPercent) 
                .Filter(_filter) 
                .Transform(trade => new TradeProxy(trade))
                .Sort(SortExpressionComparer<TradeProxy>.Descending(t => t.Timestamp))
                .ObserveOnDispatcher()
                .Bind(_data)  
                .DisposeMany() 
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
                _filter.Change(t => t.CurrencyPair.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
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

        public uint NearToMarketPercent
        {
            get { return _nearToMarketPercent; }
            set
            {
                if (_nearToMarketPercent == value) return;
                _nearToMarketPercent = value;
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