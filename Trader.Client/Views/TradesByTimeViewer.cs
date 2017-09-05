using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class TradesByTimeViewer : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<TradesByTime> _data;

        public TradesByTimeViewer(ITradeService tradeService, ISchedulerProvider schedulerProvider)
        {
            var grouperRefresher = Observable.Interval(TimeSpan.FromSeconds(1)).Select(_ => Unit.Default);


            _cleanUp = tradeService.All.Connect()
                .Group(trade =>
                       {
                           var diff = DateTime.Now.Subtract(trade.Timestamp);
                           if (diff.TotalSeconds <= 60) return TimePeriod.LastMinute;
                           if (diff.TotalMinutes <= 60) return TimePeriod.LastHour;
                           return TimePeriod.Older;
                       }, grouperRefresher)
                .Transform(group => new TradesByTime(group, schedulerProvider))
                .Sort(SortExpressionComparer<TradesByTime>.Ascending(t => t.Period))
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(out _data)
                .DisposeMany()
                .Subscribe();
        }

        public ReadOnlyObservableCollection<TradesByTime> Data => _data;

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}