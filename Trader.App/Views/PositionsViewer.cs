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
        private readonly IObservableCollection<CurrencyPairPosition> _data = new ObservableCollectionExtended<CurrencyPairPosition>();
        private readonly IDisposable _cleanUp;

        public PositionsViewer(ITradeService tradeService, ISchedulerProvider schedulerProvider)
        {
            _cleanUp = tradeService.Live.Connect()
                .Group(trade => trade.CurrencyPair)
                .Transform(group => new CurrencyPairPosition(group))
                .Sort(SortExpressionComparer<CurrencyPairPosition>.Ascending(t => t.CurrencyPair))
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(_data)
                .DisposeMany()
                .Subscribe();
        }

        public IObservableCollection<CurrencyPairPosition> Data
        {
            get { return _data; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
