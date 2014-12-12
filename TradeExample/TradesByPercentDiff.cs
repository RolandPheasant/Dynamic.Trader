using System;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Operators;
using TradeExample.Annotations;
using TradeExample.Infrastucture;

namespace TradeExample
{
    public class TradesByPercentDiff: IDisposable, IEquatable<TradesByPercentDiff>
    {
        private readonly IGroup<Trade, long, int> _group;
        private readonly IObservableCollection<TradeProxy> _data;
        private readonly IDisposable _cleanUp;
        private readonly int _percentBand;

        public TradesByPercentDiff([NotNull] IGroup<Trade, long, int> @group, 
            ISchedulerProvider schedulerProvider)
        {
            if (@group == null) throw new ArgumentNullException("group");
            _group = @group;
            _percentBand = @group.Key;

            _data = new ObservableCollectionExtended<TradeProxy>();

            _cleanUp = @group.Cache.Connect()
                        .Transform(trade => new TradeProxy(trade))
                        .Sort(SortExpressionComparer<TradeProxy>.Descending(p => p.Timestamp),SortOptimisations.ComparesImmutableValuesOnly,500)
                        .ObserveOn(schedulerProvider.Dispatcher)
                        .Bind(_data)
                        .Subscribe();
        }

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

        public int PercentBand
        {
            get { return _percentBand; }
        }

        public int PercentBandUpperBound
        {
            get { return _group.Key + 1; }
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