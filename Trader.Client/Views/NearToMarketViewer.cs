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
    public class NearToMarketViewer : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<TradeProxy> _data;
        private decimal _nearToMarketPercent=0.01M;

        public NearToMarketViewer(INearToMarketService nearToMarketService, ISchedulerProvider schedulerProvider)
        {
            _cleanUp = nearToMarketService.Query(() => NearToMarketPercent) 
                .Transform(trade => new TradeProxy(trade))
                .Sort(SortExpressionComparer<TradeProxy>.Descending(t => t.Timestamp))
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(out _data)  
                .DisposeMany() 
                .Subscribe();
        }

        public decimal NearToMarketPercent
        {
            get { return _nearToMarketPercent; }
            set { SetAndRaise(ref _nearToMarketPercent, value); }
        }

        public ReadOnlyObservableCollection<TradeProxy> Data
        {
            get { return _data; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}