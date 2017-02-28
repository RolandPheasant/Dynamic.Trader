using System;
using System.Collections.ObjectModel;
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
        private readonly ReadOnlyObservableCollection<CurrencyPairPosition> _data;
        private readonly IDisposable _cleanUp;

        public PositionsViewer(ITradeService tradeService, ISchedulerProvider schedulerProvider)
        {
            _cleanUp = tradeService.Live.Connect()
                .Group(trade => trade.CurrencyPair)
                .Transform(group => new CurrencyPairPosition(group))
                .Sort(SortExpressionComparer<CurrencyPairPosition>.Ascending(t => t.CurrencyPair))
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(out _data)
                .DisposeMany()
                .Subscribe();
        }

        public ReadOnlyObservableCollection<CurrencyPairPosition> Data => _data;

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
