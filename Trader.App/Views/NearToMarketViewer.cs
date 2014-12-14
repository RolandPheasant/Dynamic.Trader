using System;
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
        private readonly IObservableCollection<TradeProxy> _data = new ObservableCollectionExtended<TradeProxy>();
        private uint _nearToMarketPercent=5;

        public NearToMarketViewer(INearToMarketService nearToMarketService)
        {
            _cleanUp = nearToMarketService.Query(() => NearToMarketPercent) 
                .Transform(trade => new TradeProxy(trade))
                .Sort(SortExpressionComparer<TradeProxy>.Descending(t => t.Timestamp))
                .ObserveOnDispatcher()
                .Bind(_data)  
                .DisposeMany() 
                .Subscribe();
        }

        public uint NearToMarketPercent
        {
            get { return _nearToMarketPercent; }
            set
            {
                if (_nearToMarketPercent == value) return;
                _nearToMarketPercent = value;
                OnPropertyChanged();
            }
        }

        public IObservableCollection<TradeProxy> Data
        {
            get { return _data; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}