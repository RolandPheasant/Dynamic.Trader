using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Trader.Domain.Infrastucture;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class TradesByPercentViewer : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<Domain.Model.TradesByPercentDiff> _data;

        public TradesByPercentViewer(INearToMarketService nearToMarketService, ISchedulerProvider schedulerProvider, ILogger logger)
        {
            var locker = new object();
            var grouperRefresher = Observable.Interval(TimeSpan.FromSeconds(1))
                .Synchronize(locker)
                .Select(_=> Unit.Default);

            _cleanUp = nearToMarketService.Query(() => 4)
                .Synchronize(locker)
                .Group(trade => (int) Math.Truncate(Math.Abs(trade.PercentFromMarket)), grouperRefresher)
                .Transform(group => new Domain.Model.TradesByPercentDiff(group, schedulerProvider, logger))
                .Sort(SortExpressionComparer<Domain.Model.TradesByPercentDiff>.Ascending(t => t.PercentBand),SortOptimisations.ComparesImmutableValuesOnly)
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(out _data)
                .DisposeMany()
                .Subscribe(_ => { }, ex => logger.Error(ex, ex.Message));
        }

        public ReadOnlyObservableCollection<Domain.Model.TradesByPercentDiff> Data => _data;

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}