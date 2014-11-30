using System;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using TradeExample.Annotations;
using TradeExample.Infrastucture;

namespace TradeExample
{
    public class TradesByPercentDiff: IDisposable
    {
        private readonly IGroup<TradeProxy, long, int> _group;
        private readonly IObservableCollection<TradeProxy> _data;
        private readonly IDisposable _cleanUp;

        public TradesByPercentDiff([NotNull] IGroup<TradeProxy, long, int> @group, 
            ISchedulerProvider schedulerProvider)
        {
            if (@group == null) throw new ArgumentNullException("group");
            _group = @group;

            _data = new ObservableCollectionExtended<TradeProxy>();

            _cleanUp = _group.Cache.Connect()
                .Sort(SortExpressionComparer<TradeProxy>.Descending(p => p.Timestamp))
                .ObserveOn(schedulerProvider.Dispatcher)
                .Bind(_data)
                .Subscribe();
        }

        public int PercentBand
        {
            get { return _group.Key; }
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