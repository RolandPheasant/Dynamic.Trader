using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using TradeExample.Infrastucture;

namespace TradeExample
{
    public class TradesByPercentViewer : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly IDisposable _cleanUp;
        private readonly IObservableCollection<TradesByPercentDiff> _data = new ObservableCollectionExtended<TradesByPercentDiff>();

        public TradesByPercentViewer(INearToMarketService nearToMarketService, ISchedulerProvider schedulerProvider)
        {
            _schedulerProvider = schedulerProvider;

            var groupController = new GroupController();

            var grouperRefresher = Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ => groupController.RefreshGroup());

            var loader = nearToMarketService.Query(() => 6)
                .Group(trade => (int)Math.Truncate(Math.Abs(trade.PercentFromMarket)), groupController)
                .Transform(group => new TradesByPercentDiff(group, _schedulerProvider))
                .Sort(SortExpressionComparer<TradesByPercentDiff>.Ascending(t => t.PercentBand))
                .ObserveOn(_schedulerProvider.Dispatcher)
                .Bind(_data)
                .DisposeMany()
                .Subscribe();
            ;
            _cleanUp = new CompositeDisposable(loader, grouperRefresher);
        }

        public IObservableCollection<TradesByPercentDiff> Data
        {
            get { return _data; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}