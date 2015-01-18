using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using DynamicData.Operators;
using DynamicData.PLinq;
using ReactiveUI;
using Trader.Domain.Model;
using Trader.Domain.Services;
using DynamicData.ReactiveUI;

namespace Trader.Client.Views
{
    public class RxUiViewer : ReactiveObject, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly ReactiveList<TradeProxy> _data = new ReactiveList<TradeProxy>();
        private readonly FilterController<Trade> _filter = new FilterController<Trade>();
        private string _searchText;

        public RxUiViewer(ITradeService tradeService)
        {
            var filterApplier = this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Subscribe(_ => ApplyFilter());
           
            ApplyFilter();

            var loader = tradeService.Trades
                .Connect(trade => trade.Status == TradeStatus.Live) //prefilter live trades only
                .Filter(_filter) // apply user filter
                .Transform(trade => new TradeProxy(trade), new ParallelisationOptions(ParallelType.Ordered, 5)) 
               //.SubscribeMany(proxy=>Disposable.Empty)
                .Sort(SortExpressionComparer<TradeProxy>.Descending(t => t.Timestamp), SortOptimisations.ComparesImmutableValuesOnly)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(_data)    // update observable collection bindings
                .DisposeMany()  //since TradeProxy is disposable dispose when no longer required
                .Subscribe();

            _cleanUp = new CompositeDisposable(loader, _filter, filterApplier);
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                _filter.ChangeToIncludeAll();
            }
            else
            {
                _filter.Change(t => t.CurrencyPair.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                    t.Customer.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }
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