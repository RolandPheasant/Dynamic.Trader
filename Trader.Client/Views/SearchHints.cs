using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using Trader.Domain.Infrastucture;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class SearchHints : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IObservableCollection<string> _hints = new ObservableCollectionExtended<string>();
        private readonly IDisposable _cleanUp;
        private string _searchText;

        public SearchHints(ITradeService tradeService, ISchedulerProvider schedulerProvider)
        {
            //instaniate a filter controller so we can change the filter any time
            var filter = new FilterController<string>();

            //build a predicate when SeatchText changes
            var filterApplier = this.ObservePropertyValue(t => t.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(propargs => BuildFilter(propargs.Value))
                .Subscribe(filter.Change);

            //share the connection
            var shared = tradeService.All.Connect().Publish();
            //distinct list of customers
            var customers = shared.DistinctValues(trade => trade.Customer);
            //distinct list of currency pairs
            var currencypairs = shared.DistinctValues(trade => trade.CurrencyPair);

            //create single list of all customers all items in currency ypairs
            var loader = customers.Or(currencypairs)
                .Filter(filter)     //filter strings
                .Sort(SortExpressionComparer<string>.Ascending(str=>str))
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(_hints)       //bind to hints list
                .Subscribe();

            _cleanUp = new CompositeDisposable(loader, filter, shared.Connect(), filterApplier);
        }

        private Func<string, bool> BuildFilter(string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) return trade => true;

            return str => str.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        str.Contains(searchText, StringComparison.OrdinalIgnoreCase);
        }

        public string SearchText
        {
            get { return _searchText; }
            set { SetAndRaise(ref _searchText, value); }
        }

        public IObservableCollection<string> Hints
        {
            get { return _hints; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}