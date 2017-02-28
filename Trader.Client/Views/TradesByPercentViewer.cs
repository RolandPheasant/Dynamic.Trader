using System;
using System.Collections.ObjectModel;
using System.Reactive;
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
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<Domain.Model.TradesByPercentDiff> _data;

        public TradesByPercentViewer(INearToMarketService nearToMarketService, ISchedulerProvider schedulerProvider)
        {
            var grouperRefresher = Observable.Interval(TimeSpan.FromSeconds(1)).Select(_=> Unit.Default);

            _cleanUp = nearToMarketService.Query(() => 6)
                .Group(trade => (int)Math.Truncate(Math.Abs(trade.PercentFromMarket)), grouperRefresher)
                .Transform(group => new Domain.Model.TradesByPercentDiff(group, schedulerProvider))
                .Sort(SortExpressionComparer<Domain.Model.TradesByPercentDiff>.Ascending(t => t.PercentBand),SortOptimisations.ComparesImmutableValuesOnly)
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(out _data)
                .DisposeMany()
                .Subscribe();
        }

        public ReadOnlyObservableCollection<Domain.Model.TradesByPercentDiff> Data => _data;

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}