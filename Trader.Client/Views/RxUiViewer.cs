using System;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.PLinq;
using ReactiveUI;
using Trader.Domain.Model;
using Trader.Domain.Services;
using DynamicData.ReactiveUI;

namespace Trader.Client.Views
{
    public class RxUiViewer : ReactiveObject, IDisposable
    {
        //this is the target list which we will populate from the dynamic data stream
        private readonly ReactiveList<TradeProxy> _data = new ReactiveList<TradeProxy>();
        private readonly IDisposable _cleanUp;
        private string _searchText;

        public RxUiViewer(ITradeService tradeService)
        {
            //Change the filter when the user entered search text changes
            var filter = this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(BuildFilter);

            _cleanUp = tradeService.All
                .Connect(trade => trade.Status == TradeStatus.Live) //prefilter live trades only
                .Filter(filter)    // apply user filter
                //if targetting Net4 or Net45 platform can use parallelisation for transforms 'cause it's quicker
                .Transform(trade => new TradeProxy(trade), new ParallelisationOptions(ParallelType.Ordered, 5))
                .Sort(SortExpressionComparer<TradeProxy>.Descending(t => t.Timestamp), SortOptimisations.ComparesImmutableValuesOnly)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(_data)        //bind the results to the ReactiveList 
                .DisposeMany()      //since TradeProxy is disposable dispose when no longer required
                .Subscribe();
        }


        private Func<Trade, bool> BuildFilter(string searchText)
        {
            if (string.IsNullOrEmpty(SearchText))
                return le => true;

            return t => t.CurrencyPair.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        t.Customer.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        }

        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        public IReadOnlyReactiveList<TradeProxy> Data
        {
            get { return _data; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}