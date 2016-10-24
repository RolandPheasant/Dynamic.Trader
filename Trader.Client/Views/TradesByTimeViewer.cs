using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class TradesByTimeViewer : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<TradesByTime> _data;

        public TradesByTimeViewer(ITradeService tradeService, ISchedulerProvider schedulerProvider)
        {
            _schedulerProvider = schedulerProvider;

            var groupController = new GroupController();
            var grouperRefresher = Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ => groupController.RefreshGroup());

            var loader = tradeService.All.Connect()
                .Group(trade =>
                       {
                           var diff = DateTime.Now.Subtract(trade.Timestamp);
                           if (diff.TotalSeconds <= 60) return TimePeriod.LastMinute;
                           return diff.TotalMinutes <= 60 ? TimePeriod.LastHour : TimePeriod.Older;
                       }, groupController)
                .Transform(group => new TradesByTime(group, _schedulerProvider))
                .Sort(SortExpressionComparer<TradesByTime>.Ascending(t => t.Period))
                .ObserveOn(_schedulerProvider.MainThread)
                .Bind(out _data)
                .DisposeMany()
                .Subscribe();
            
            _cleanUp = new CompositeDisposable(loader, grouperRefresher);
        }

        public ReadOnlyObservableCollection<TradesByTime> Data => _data;

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}