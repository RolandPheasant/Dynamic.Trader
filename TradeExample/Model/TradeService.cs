using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
using DynamicData.Operators;
using TradeExample.Annotations;
using TradeExample.Infrastucture;
using TradeExample.Services;

namespace TradeExample.Model
{
    public class TradesByTime : IDisposable, IEquatable<TradesByTime>
    {
        private readonly IObservableCollection<TradeProxy> _data;
        private readonly IDisposable _cleanUp;
        private readonly TimePeriod _period;

        public TradesByTime([NotNull] IGroup<Trade, long, TimePeriod> @group,
            ISchedulerProvider schedulerProvider)
        {
            if (@group == null) throw new ArgumentNullException("group");
            _period = @group.Key;

            _data = new ObservableCollectionExtended<TradeProxy>();

            _cleanUp = @group.Cache.Connect()
                .Transform(trade => new TradeProxy(trade))
                .Sort(SortExpressionComparer<TradeProxy>.Descending(p => p.Timestamp), SortOptimisations.ComparesImmutableValuesOnly)
                .ObserveOn(schedulerProvider.Dispatcher)
                .Bind(_data)
                .DisposeMany()
                .Subscribe();
        }

        #region Equality

        public bool Equals(TradesByTime other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _period == other._period;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TradesByTime)obj);
        }

        public override int GetHashCode()
        {
            return (int)_period;
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

        public TimePeriod Period
        {
            get { return _period; }
        }

        public string Description
        {
            get
            {
                switch (Period)
                {
                    case TimePeriod.LastMinute:
                        return "Last Minute";
                    case TimePeriod.LastHour:
                        return "Last Hour"; ;
                    case TimePeriod.Older:
                        return "Old";
                    default:
                        return "Unknown";
                }
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

    public class TradeService : ITradeService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly TradeGenerator _tradeGenerator;
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly ISourceCache<Trade, long> _tradesSource;
        private readonly IObservableCache<Trade, long> _tradesCache;
        private readonly IDisposable _cleanup;

        public TradeService(ILogger logger,TradeGenerator tradeGenerator, ISchedulerProvider schedulerProvider)
        {
            _logger = logger;
            _tradeGenerator = tradeGenerator;
            _schedulerProvider = schedulerProvider;

            //construct a cache specifying that the unique key is Trade.Id
            _tradesSource = new SourceCache<Trade, long>(trade => trade.Id);

            //call AsObservableCache() to hide the update methods as we are exposing the cache
            _tradesCache = _tradesSource.AsObservableCache();

            //code to emulate an external trade provider
            var tradeLoader = GenerateTradesAndMaintainCache();

            _cleanup = new CompositeDisposable(_tradesCache, _tradesSource, tradeLoader);
        }


        private IDisposable GenerateTradesAndMaintainCache()
        {
            //bit of code to generate trades
            var random = new Random();

            //initally load some trades 
            _tradesSource.AddOrUpdate(_tradeGenerator.Generate(10000, true));

            Func<TimeSpan> randomInterval = () =>
                                            {
                                                var ms = random.Next(150, 5000);
                                                return TimeSpan.FromMilliseconds(ms);
                                            };

            // create a random number of trades at a random interval
            var tradeGenerator = _schedulerProvider.TaskPool
                            .ScheduleRecurringAction(randomInterval, () =>
                            {
                                var number = random.Next(1, 5);
                                _logger.Info("Adding {0} trades", number);
                                var trades = _tradeGenerator.Generate(number);
                                _tradesSource.AddOrUpdate(trades);
                            });
           
            // close a random number of trades at a random interval
            var tradeCloser = _schedulerProvider.TaskPool
                .ScheduleRecurringAction(randomInterval, () =>
                {
                    
                    
                    
                    var number = random.Next(1, 5);
                    _logger.Info("Closing {0} trades", number);
                    
                    
                    _tradesSource.BatchUpdate(updater =>
                                              {
                                                  var trades = updater.Items
                                                    .Where(trade => trade.Status == TradeStatus.Live)
                                                    .OrderBy(t => Guid.NewGuid()).Take(number).ToArray();

                                                  var toClose = trades
                                                      .Select(trade => new Trade(trade.Id, trade.Customer, trade.CurrencyPair, TradeStatus.Closed, trade.MarketPrice));

                                                  _tradesSource.AddOrUpdate(toClose);
                                              });

                });

            return new CompositeDisposable(tradeGenerator, tradeCloser);

        }


        public IObservableCache<Trade, long> Trades
        {
            get { return _tradesCache; }
        }


        public void Dispose()
        {
            _cleanup.Dispose();
        }
    }
}