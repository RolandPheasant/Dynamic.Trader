using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using DynamicData.Operators;
using Trader.Domain.Infrastucture;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class TradesByPercentViewer : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<Domain.Model.TradesByPercentDiff> _data;

        public TradesByPercentViewer(INearToMarketService nearToMarketService, ISchedulerProvider schedulerProvider)
        {
            _schedulerProvider = schedulerProvider;

            var groupController = new GroupController();

            var grouperRefresher = Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ => groupController.RefreshGroup());

            var loader = nearToMarketService.Query(() => 6)
                .Group(trade => (int)Math.Truncate(Math.Abs(trade.PercentFromMarket)), groupController)
                .Transform(group => new Domain.Model.TradesByPercentDiff(group, _schedulerProvider))
                .Sort(SortExpressionComparer<Domain.Model.TradesByPercentDiff>.Ascending(t => t.PercentBand),SortOptimisations.ComparesImmutableValuesOnly)
                .ObserveOn(_schedulerProvider.MainThread)
                .Bind(out _data)
                .DisposeMany()
                .Subscribe();

            _cleanUp = new CompositeDisposable(loader, grouperRefresher);
        }

        public ReadOnlyObservableCollection<Domain.Model.TradesByPercentDiff> Data => _data;

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}