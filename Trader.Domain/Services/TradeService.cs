using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Kernel;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class TradeService : ITradeService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly TradeGenerator _tradeGenerator;
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly ISourceCache<Trade, long> _tradesSource;
        private readonly IDisposable _cleanup;

        public TradeService(ILogger logger,TradeGenerator tradeGenerator, ISchedulerProvider schedulerProvider)
        {
            _logger = logger;
            _tradeGenerator = tradeGenerator;
            _schedulerProvider = schedulerProvider;

            //construct a cache specifying that the primary key is Trade.Id
            _tradesSource = new SourceCache<Trade, long>(trade => trade.Id);

            //call AsObservableCache() to hide the update methods as we are exposing the cache
            All = _tradesSource.AsObservableCache();

            //create a derived cache  
            Live = _tradesSource.Connect(trade => trade.Status == TradeStatus.Live).AsObservableCache();

            //code to emulate an external trade provider
            var tradeLoader = GenerateTradesAndMaintainCache();

            //expire closed items from the cache ro avoid unbounded data
            var expirer = _tradesSource
                .ExpireAfter(t => t.Status == TradeStatus.Closed ? TimeSpan.FromMinutes(1) : (TimeSpan?) null, TimeSpan.FromMinutes(1), schedulerProvider.TaskPool)
                .Subscribe(x => _logger.Info("{0} filled trades have been removed from memory", x.Count()));

            //log changes
            var loggerWriter = LogChanges();

            _cleanup = new CompositeDisposable(All, _tradesSource, tradeLoader, loggerWriter, expirer);
        }
        
        private IDisposable GenerateTradesAndMaintainCache()
        {
            //bit of code to generate trades
            var random = new Random();

            //initally load some trades 
            _tradesSource.AddOrUpdate(_tradeGenerator.Generate(10000, true));

            Func<TimeSpan> randomInterval = () => TimeSpan.FromMilliseconds(random.Next(2500, 15000));
                                             

            // create a random number of trades at a random interval
            var tradeGenerator = _schedulerProvider.TaskPool
                            .ScheduleRecurringAction(randomInterval, () =>
                            {
                                var number = random.Next(1,5);
                                var trades = _tradeGenerator.Generate(number);
                                _tradesSource.AddOrUpdate(trades);
                            });
           
            // close a random number of trades at a random interval
            var tradeCloser = _schedulerProvider.TaskPool
                .ScheduleRecurringAction(randomInterval, () =>
                {
                    var number = random.Next(1, 2);
                    _tradesSource.Edit(innerCache =>
                                              {
                                                  var toClose = innerCache.Items
                                                    .Where(trade => trade.Status == TradeStatus.Live)
                                                    .OrderBy(t => Guid.NewGuid())
                                                    .Take(number)
                                                    .Select(trade => new Trade(trade, TradeStatus.Closed))
                                                    .ToArray();

                                                  innerCache.AddOrUpdate(toClose);
                                              });
                });

            return new CompositeDisposable(tradeGenerator, tradeCloser);
        }

        private IDisposable LogChanges()
        {
            const string messageTemplate = "{0} {1} {2} ({4}). Status = {3}";
            return All.Connect().Skip(1)
                            .WhereReasonsAre(ChangeReason.Add,ChangeReason.Update)
                            .Cast(trade => string.Format(messageTemplate,
                                                    trade.BuyOrSell,
                                                    trade.Amount,
                                                    trade.CurrencyPair,
                                                    trade.Status,
                                                    trade.Customer))
                            .ForEachChange(change=>_logger.Info(change.Current))
                            .Subscribe();

        }

        public IObservableCache<Trade, long> All { get; }

        public IObservableCache<Trade, long> Live { get; }

        public void Dispose()
        {
            _cleanup.Dispose();
        }
    }
}