using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
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
            //build observable predicate from search text
            var filter = this.WhenValueChanged(t => t.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(BuildFilter);

            //build observable sort comparer
            var sort = SortParameters.WhenValueChanged(t => t.SelectedItem)
                .Select(prop => prop.Comparer)
                .ObserveOn(schedulerProvider.TaskPool);

            //build observable comparer
            var currentPageChanged = PageParameters.WhenValueChanged(p => p.CurrentPage);
            var pageSizeChanged = PageParameters.WhenValueChanged(p => p.PageSize);
            var pager = currentPageChanged.CombineLatest(pageSizeChanged,(page, size) => new PageRequest(page, size))
                .StartWith(new PageRequest(1, 25))
                .DistinctUntilChanged()
                .Sample(TimeSpan.FromMilliseconds(100));


            // filter, sort, page and bind to loaded data
            _cleanUp = tradeService.All.Connect()
                .Filter(filter) // apply user filter
                .Transform(trade => new TradeProxy(trade), new ParallelisationOptions(ParallelType.Ordered, 5))
                .Sort(sort, SortOptimisations.ComparesImmutableValuesOnly)
                .Page(pager)
                .ObserveOn(schedulerProvider.MainThread)
                .Do(changes => _pageParameters.Update(changes.Response))
                .Bind(out _data)        // update observable collection bindings
                .DisposeMany()          // dispose when no longer required
                .Subscribe();
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