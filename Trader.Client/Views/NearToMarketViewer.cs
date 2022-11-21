using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views;

public class NearToMarketViewer : AbstractNotifyPropertyChanged, IDisposable
{
    private readonly IDisposable _cleanUp;
    private readonly ReadOnlyObservableCollection<TradeProxy> _data;
    private double _nearToMarketPercent = 0.05D;

    public NearToMarketViewer(INearToMarketService nearToMarketService, ISchedulerProvider schedulerProvider, ILogger logger)
    {
        _cleanUp = nearToMarketService.Query(() => (decimal)NearToMarketPercent)
            .Transform(trade => new TradeProxy(trade))
            .Sort(SortExpressionComparer<TradeProxy>.Descending(t => t.Timestamp))
            .ObserveOn(schedulerProvider.MainThread)
            .Bind(out _data)
            .DisposeMany()
            .Subscribe(_ => { }, ex => logger.Error(ex, "Error in near to market viewer"));
    }

    public double NearToMarketPercent
    {
        get => _nearToMarketPercent;
        set => SetAndRaise(ref _nearToMarketPercent, value);
    }

    public ReadOnlyObservableCollection<TradeProxy> Data => _data;

    public void Dispose()
    {
        _cleanUp.Dispose();
    }
}