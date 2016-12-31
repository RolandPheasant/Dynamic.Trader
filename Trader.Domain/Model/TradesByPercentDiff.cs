using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Operators;
using TradeExample.Annotations;
using Trader.Domain.Infrastucture;

namespace Trader.Domain.Model
{
    public class TradesByPercentDiff: IDisposable, IEquatable<TradesByPercentDiff>
    {
        private readonly IGroup<Trade, long, int> _group;
        private readonly ReadOnlyObservableCollection<TradeProxy> _data;
        private readonly IDisposable _cleanUp;
        private readonly int _percentBand;

        public TradesByPercentDiff([NotNull] IGroup<Trade, long, int> group,[NotNull] ISchedulerProvider schedulerProvider)
        {
            if (group == null) throw new ArgumentNullException(nameof(@group));
            if (schedulerProvider == null) throw new ArgumentNullException(nameof(schedulerProvider));

            _group = group;
            _percentBand = group.Key;
           
            _cleanUp = group.Cache.Connect()
                        .Transform(trade => new TradeProxy(trade))
                        .Sort(SortExpressionComparer<TradeProxy>.Descending(p => p.Timestamp),SortOptimisations.ComparesImmutableValuesOnly,500)
                        .ObserveOn(schedulerProvider.MainThread)
                        .Bind(out _data)
                        .DisposeMany()
                        .Subscribe();
        }

        public int PercentBand => _percentBand;

        public int PercentBandUpperBound => _group.Key + 1;

        public ReadOnlyObservableCollection<TradeProxy> Data => _data;


        #region Equality

        public bool Equals(TradesByPercentDiff other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _percentBand == other._percentBand;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TradesByPercentDiff) obj);
        }

        public override int GetHashCode()
        {
            return _percentBand;
        }

        public static bool operator ==(TradesByPercentDiff left, TradesByPercentDiff right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TradesByPercentDiff left, TradesByPercentDiff right)
        {
            return !Equals(left, right);
        }

        #endregion


        public void Dispose()
        {
            _cleanUp.Dispose();
        }

    }
}