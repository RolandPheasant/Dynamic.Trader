using System;
using System.Reactive.Linq;
using DynamicData;
using TradeExample.Annotations;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class NearToMarketService : INearToMarketService
    {
        private readonly ITradeService _tradeService;

        public NearToMarketService([NotNull] ITradeService tradeService)
        {
            if (tradeService == null) throw new ArgumentNullException(nameof(tradeService));
            _tradeService = tradeService;
        }

        public IObservable<IChangeSet<Trade, long>> Query(Func<decimal> percentFromMarket)
        {
            if (percentFromMarket == null) throw new ArgumentNullException(nameof(percentFromMarket));

            return Observable.Create<IChangeSet<Trade, long>>
                (observer =>
                 {
                     var locker = new object();

                     Func<Trade, bool> predicate = t =>
                     {
                         var near = percentFromMarket();
                         return Math.Abs(t.PercentFromMarket) <= near;
                     };


                     //re-evaluate filter periodically
                     var reevaluator = Observable.Interval(TimeSpan.FromMilliseconds(250))
                         .Synchronize(locker)
                         .Select(_ => predicate);

                     //filter on live trades matching % specified
                     return _tradeService.All
                         .Connect(trade=>trade.Status==TradeStatus.Live)
                         .Synchronize(locker)
                         .Filter(reevaluator)
                         .SubscribeSafe(observer);
                 });
        }
    }
}