using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using TradeExample.Annotations;
using Trader.Domain.Infrastucture;

namespace Trader.Domain.Model;

public class TradesByTime : IDisposable, IEquatable<TradesByTime>
{
    private readonly IDisposable _cleanUp;
    private readonly ReadOnlyObservableCollection<TradeProxy> _data;

    public TradesByTime([NotNull] IGroup<Trade, long, TimePeriod> group,
        ISchedulerProvider schedulerProvider)
    {
        Period = group?.Key ?? throw new ArgumentNullException(nameof(group));

        _cleanUp = group.Cache.Connect()
            .Transform(trade => new TradeProxy(trade))
            .Sort(SortExpressionComparer<TradeProxy>.Descending(p => p.Timestamp), SortOptimisations.ComparesImmutableValuesOnly)
            .ObserveOn(schedulerProvider.MainThread)
            .Bind(out _data)
            .DisposeMany()
            .Subscribe();
    }


    public TimePeriod Period { get; }

    public string Description
    {
        get
        {
            switch (Period)
            {
                case TimePeriod.LastMinute:
                    return "Last Minute";
                case TimePeriod.LastHour:
                    return "Last Hour";
                    ;
                case TimePeriod.Older:
                    return "Old";
                default:
                    return "Unknown";
            }
        }
    }

    public ReadOnlyObservableCollection<TradeProxy> Data => _data;

    public void Dispose()
    {
        _cleanUp.Dispose();
    }

    #region Equality

    public bool Equals(TradesByTime other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Period == other.Period;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((TradesByTime) obj);
    }

    public override int GetHashCode()
    {
        return (int) Period;
    }

    public static bool operator ==(TradesByTime left, TradesByTime right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TradesByTime left, TradesByTime right)
    {
        return !Equals(left, right);
    }

    #endregion
}