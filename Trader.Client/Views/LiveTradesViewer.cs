using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using DynamicData.Operators;
using DynamicData.PLinq;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class LiveTradesViewer :AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly ILogger _logger;
        private readonly SearchHints _searchHints;
        private readonly IDisposable _cleanUp;
        private readonly IObservableCollection<TradeProxy> _data = new ObservableCollectionExtended<TradeProxy>();

        public LiveTradesViewer(ILogger logger,ITradeService tradeService,SearchHints searchHints)
        {
            _logger = logger;
            _searchHints = searchHints;

            var filter =  new FilterController<Trade>(trade=>true);
            var filterApplier = SearchHints.ObservePropertyValue(t => t.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(propargs=>BuildFilter(propargs.Value))
                .Subscribe(filter.Change);

            var loader = tradeService.All
                .Connect(trade => trade.Status == TradeStatus.Live) //prefilter live trades only
                .Filter(filter) // apply user filter
                //if using dotnet 4.5 can parallelise 'case it's quicler
                .Transform(trade => new TradeProxy(trade),new ParallelisationOptions(ParallelType.Ordered,5))
                .Sort(SortExpressionComparer<TradeProxy>.Descending(t => t.Timestamp),SortOptimisations.ComparesImmutableValuesOnly)
                .ObserveOnDispatcher()
                .Bind(_data)   // update observable collection bindings
                .DisposeMany() //since TradeProxy is disposable dispose when no longer required
                .Subscribe();

            _cleanUp = new CompositeDisposable(loader, filter, filterApplier, searchHints);
        }

        private Func<Trade, bool> BuildFilter(string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) return trade => true;

            return t => t.CurrencyPair.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                            t.Customer.Contains(searchText, StringComparison.OrdinalIgnoreCase);
        }


        public IObservableCollection<TradeProxy> Data
        {
            get { return _data; }
        }

        public SearchHints SearchHints
        {
            get { return _searchHints; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}