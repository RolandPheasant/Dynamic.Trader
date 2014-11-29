using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using DynamicData.Kernel;
using TradeExample.Annotations;
using TradeExample.Infrastucture;

namespace TradeExample
{

    public enum TimeLineGroup
    {
        JustNow,
        Recent,
        Old
    }

    public class TradesByTimeLine
    {
        private readonly IGroup<TradeProxy, long, TimeLineGroup> _group;

        public TradesByTimeLine([NotNull] IGroup<TradeProxy, long, TimeLineGroup> @group)
        {
            if (@group == null) throw new ArgumentNullException("group");
            _group = @group;
        }

        public TimeLineGroup Key
        {
            get { return _group.Key; }
        }

        public IObservableCache<TradeProxy, long> Cache
        {
            get { return _group.Cache; }
        }
    }

    public class GroupByTimeViewer : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IDisposable _cleanUp;
        private readonly IObservableCollection<TradesByTimeLine> _data = new ObservableCollectionExtended<TradesByTimeLine>();
        private readonly FilterController<Trade> _filter = new FilterController<Trade>();
        private string _searchText;

        public GroupByTimeViewer(ILogger logger, ITradeService tradeService)
        {
            _logger = logger;

            var filterApplier = this.ObserveChanges()
                .Sample(TimeSpan.FromMilliseconds(250))
                .Subscribe(_ => ApplyFilter());

            ApplyFilter();

            var groupController = new GroupController();

            var loader = tradeService.Trades.Connect()
                .Filter(_filter) // apply user filter
                .Transform(trade => new TradeProxy(trade))
                .Group(trade =>
                       {
                           var timeDiff = DateTime.Now.Subtract(trade.Timestamp);
                           if (timeDiff.TotalSeconds < 20) return TimeLineGroup.JustNow;
                           if (timeDiff.TotalMinutes < 1) return TimeLineGroup.Recent;
                           return TimeLineGroup.Old;


                       },groupController)


                .Transform(group => new TradesByTimeLine(group))

                .Sort(SortExpressionComparer<TradesByTimeLine>.Descending(t => t.Key))
                .ObserveOnDispatcher()
                .Bind(_data)   // update observable collection bindings
                .DisposeMany() //since TradeProxy is disposable dispose when no longer required
                .Subscribe();


          // groupController.RefreshGroup();
            _cleanUp = new CompositeDisposable(loader, _filter, filterApplier);
        }

        private void ApplyFilter()
        {
            _logger.Info("Applying filter");
            if (string.IsNullOrEmpty(SearchText))
            {
                _filter.ChangeToIncludeAll();
            }
            else
            {
                _filter.Change(t => t.CurrencyPair.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                    t.Customer.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }
            _logger.Info("Applied filter");
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public IObservableCollection<TradesByTimeLine> Data
        {
            get { return _data; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}