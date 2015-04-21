using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ILogger _logger;
        private readonly IDisposable _cleanUp;
        private readonly IObservableCollection<TradeProxy> _data = new ObservableCollectionExtended<TradeProxy>();
        private readonly PageParameterData _pageParameters = new PageParameterData(1,25);
        private readonly  SortParameterData _sortParameters = new SortParameterData();
        
        private string _searchText;

        public PagedDataViewer(ILogger logger, ITradeService tradeService, ISchedulerProvider schedulerProvider)
        {
            _logger = logger;

            //watch for filter changes and change filter 
            var filterController = new FilterController<Trade>(trade => true);
            var filterApplier = this.ObservePropertyValue(t => t.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(propargs => BuildFilter(propargs.Value))
                .Subscribe(filterController.Change);

            //watch for changes to sort and apply when necessary
            var sortContoller = new SortController<TradeProxy>(SortExpressionComparer<TradeProxy>.Ascending(proxy=>proxy.Id));
            var sortChange = SortParameters.ObservePropertyValue(t => t.SelectedItem).Select(prop=>prop.Value.Comparer)
                    .ObserveOn(schedulerProvider.TaskPool)
                    .Subscribe(sortContoller.Change);
            
            //watch for page changes and change filter 
            var pageController = new PageController();
            var currentPageChanged = PageParameters.ObservePropertyValue(p => p.CurrentPage).Select(prop => prop.Value);
            var pageSizeChanged = PageParameters.ObservePropertyValue(p => p.PageSize).Select(prop => prop.Value);
            var pageChanger = currentPageChanged.CombineLatest(pageSizeChanged,
                                (page, size) => new PageRequest(page, size))
                                .DistinctUntilChanged()
                                .Sample(TimeSpan.FromMilliseconds(100))
                                .Subscribe(pageController.Change);


            var loader = tradeService.All .Connect() 
                .Filter(filterController) // apply user filter
                .Transform(trade => new TradeProxy(trade), new ParallelisationOptions(ParallelType.Ordered, 5))
                .Sort(sortContoller, SortOptimisations.ComparesImmutableValuesOnly)
                .Page(pageController)
                .ObserveOn(schedulerProvider.MainThread)
                .Do(changes => _pageParameters.Update(changes.Response))
                .Bind(_data)     // update observable collection bindings
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

        public IObservableCollection<TradeProxy> Data
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

    public class PageParameterData : AbstractNotifyPropertyChanged
    {
        private int _currentPage;
        private int _pageCount;
        private int _pageSize;
        private int _totalCount;
        private readonly Command _nextPageCommand;
        private readonly Command _previousPageCommand;

        public PageParameterData(int currentPage, int pageSize)
        {
            _currentPage = currentPage;
            _pageSize = pageSize;

            _nextPageCommand = new Command(() => CurrentPage = CurrentPage + 1, () => CurrentPage < PageCount);
            _previousPageCommand = new Command(() => CurrentPage = CurrentPage - 1, () => CurrentPage > 1);
        }


        public void Update(IPageResponse response)
        {
            CurrentPage = response.Page;
            PageSize = response.PageSize;
            PageCount = response.Pages;
            TotalCount = response.TotalSize;
            _nextPageCommand.Refresh();
            _previousPageCommand.Refresh();
        }

        public Command NextPageCommand
        {
            get { return _nextPageCommand; }
        }

        public Command PreviousPageCommand
        {
            get { return _previousPageCommand; }
        }

        public int TotalCount
        {
            get { return _totalCount; }
            private set { SetAndRaise(ref _totalCount, value); }
        }

        public int PageCount
        {
            get { return _pageCount; }
            private set { SetAndRaise(ref _pageCount, value); }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
            private set { SetAndRaise(ref _currentPage, value); }
        }


        public int PageSize
        {
            get { return _pageSize; }
            private set { SetAndRaise(ref _pageSize, value); }
        }


    }

    public class SortParameterData : AbstractNotifyPropertyChanged
    {
        private SortContainer _selectedItem;
        private readonly IEnumerable<SortContainer> _sortItems = new List<SortContainer>
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
            SelectedItem = _sortItems.First();
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
    
}