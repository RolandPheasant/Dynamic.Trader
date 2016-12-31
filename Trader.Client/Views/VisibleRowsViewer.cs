using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Operators;
using DynamicData.PLinq;
using Trader.Client.Infrastucture;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class VisibleRowsViewer : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;

        private readonly IVisibleRowsAccessor<TradeProxy> _visibleRowsAccessor = new VisibleRowsAccessor<TradeProxy>();
        private readonly ReadOnlyObservableCollection<TradeProxy> _data;

        public VisibleRowsViewer(ITradeService tradeService)
        {
            var loader = tradeService.All.Connect()
                .Transform(trade => new TradeProxy(trade), new ParallelisationOptions(ParallelType.Ordered, 5))
                .Sort(SortExpressionComparer<TradeProxy>.Descending(t => t.Timestamp), SortOptimisations.ComparesImmutableValuesOnly)
                .ObserveOnDispatcher()
                .Bind(out _data)   // update observable collection bindings
                .DisposeMany() //since TradeProxy is disposable dispose when no longer required
                .Subscribe();
            
            //NEED TO DO SOMETHING FUNKY

            var visibilityController = _visibleRowsAccessor.VisibleRows.Connect()
                                                .SubscribeMany(proxy =>
                                                {
                                                    //
                                                    return proxy.WhenValueChanged(p => p.Amount).Subscribe();
                                                }).Subscribe();

            _cleanUp = new CompositeDisposable(loader, _visibleRowsAccessor, visibilityController);
        }

        public IVisibleRowsAccessor<TradeProxy> VisibleRowsAccessor => _visibleRowsAccessor;

        public ReadOnlyObservableCollection<TradeProxy> Data => _data;


        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}