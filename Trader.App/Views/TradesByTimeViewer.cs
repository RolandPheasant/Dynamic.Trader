using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using TradeExample.Infrastucture;
using TradeExample.Model;
using TradeExample.Services;

namespace TradeExample
{

    public class TradesByTimeViewer : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly IDisposable _cleanUp;
        private readonly IObservableCollection<TradesByTime> _data = new ObservableCollectionExtended<TradesByTime>();

        public TradesByTimeViewer(ITradeService tradeService, ISchedulerProvider schedulerProvider)
        {
            _schedulerProvider = schedulerProvider;

            var groupController = new GroupController();

            var grouperRefresher = Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ => groupController.RefreshGroup());

            var loader = tradeService.Trades.Connect()
                .Group(trade =>
                       {
                           var diff = DateTime.Now.Subtract(trade.Timestamp);
                           if (diff.TotalSeconds <= 60) return TimePeriod.LastMinute;
                           if (diff.TotalMinutes <= 60) return TimePeriod.LastHour;
                           return TimePeriod.Older;
                       }, groupController)
                .Transform(group => new TradesByTime(group, _schedulerProvider))
                .Sort(SortExpressionComparer<TradesByTime>.Ascending(t => t.Period))
                .ObserveOn(_schedulerProvider.Dispatcher)
                .Bind(_data)
                .DisposeMany()
                .Subscribe();
            
            _cleanUp = new CompositeDisposable(loader, grouperRefresher);
        }


        public IObservableCollection<TradesByTime> Data
        {
            get { return _data; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}