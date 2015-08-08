using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using DynamicData.Operators;
using DynamicData.PLinq;
using Trader.Client.Infrastucture;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class PagedDataViewer : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<TradeProxy> _data;
        private readonly PageParameterData _pageParameters = new PageParameterData(1,25);
        private readonly SortParameterData _sortParameters = new SortParameterData();
        private string _searchText;

        public PagedDataViewer(ITradeService tradeService, ISchedulerProvider schedulerProvider)
        {

            //watch for filter changes and change filter 
            var filterController = new FilterController<Trade>(trade => true);
            var filterApplier = this.WhenValueChanged(t => t.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(BuildFilter)
                .Subscribe(filterController.Change);

            //watch for changes to sort and apply when necessary
            var sortContoller = new SortController<TradeProxy>(SortExpressionComparer<TradeProxy>.Ascending(proxy=>proxy.Id));
            var sortChange = SortParameters.WhenValueChanged(t => t.SelectedItem).Select(prop=>prop.Comparer)
                    .ObserveOn(schedulerProvider.TaskPool)
                    .Subscribe(sortContoller.Change);
            
            //watch for page changes and change filter 
            var pageController = new PageController();
            var currentPageChanged = PageParameters.WhenValueChanged(p => p.CurrentPage);
            var pageSizeChanged = PageParameters.WhenValueChanged(p => p.PageSize);
            var pageChanger = currentPageChanged.CombineLatest(pageSizeChanged,(page, size) => new PageRequest(page, size))
                                .DistinctUntilChanged()
                                .Sample(TimeSpan.FromMilliseconds(100))
                                .Subscribe(pageController.Change);

            // filter, sort, page and bind to loaded data
            var loader = tradeService.All .Connect() 
                .Filter(filterController) // apply user filter
                .Transform(trade => new TradeProxy(trade), new ParallelisationOptions(ParallelType.Ordered, 5))
                .Sort(sortContoller, SortOptimisations.ComparesImmutableValuesOnly)
                .Page(pageController)
                .ObserveOn(schedulerProvider.MainThread)
                .Do(changes => _pageParameters.Update(changes.Response))
                .Bind(out _data)     // update observable collection bindings
                .DisposeMany()   //since TradeProxy is disposable dispose when no longer required
                .Subscribe();

            _cleanUp = new CompositeDisposable(loader, filterController, filterApplier, sortChange,sortContoller, pageChanger, pageController);
        }

        private Func<Trade, bool> BuildFilter(string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) return trade => true;

            return t => t.CurrencyPair.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                            t.Customer.Contains(searchText, StringComparison.OrdinalIgnoreCase);
        }

        public string SearchText
        {
            get { return _searchText; }
            set { SetAndRaise(ref _searchText, value); }
        }

        public ReadOnlyObservableCollection<TradeProxy> Data
        {
            get { return _data; }
        }

        public PageParameterData PageParameters
        {
            get { return _pageParameters; }
        }

        public SortParameterData SortParameters
        {
            get { return _sortParameters; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }


    
}