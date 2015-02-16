using System;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class PositionsViewer: IDisposable
    {
        private readonly IObservableCollection<TradesByCurrencyPair> _data = new ObservableCollectionExtended<TradesByCurrencyPair>();
        private readonly IDisposable _cleanUp;

        public PositionsViewer(ITradeService tradeService, ISchedulerProvider schedulerProvider)
        {
            _cleanUp = tradeService.Live.Connect()
                .Group(trade => trade.CurrencyPair)
                .Transform(group => new TradesByCurrencyPair(group))
                .Sort(SortExpressionComparer<TradesByCurrencyPair>.Ascending(t => t.CurrencyPair))
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(_data)
                .DisposeMany()
                .Subscribe();
        }

        public IObservableCollection<TradesByCurrencyPair> Data
        {
            get { return _data; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
