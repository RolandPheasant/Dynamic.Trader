using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class TradePriceUpdateJob: IDisposable
    {
        private readonly IDisposable _job;

        public TradePriceUpdateJob(ITradeService tradeService, IMarketDataService marketDataService)
        {
            _job = tradeService.All
                .Connect(trade => trade.Status == TradeStatus.Live)
                .Group(trade => trade.CurrencyPair)
                .SubscribeMany(groupedData =>
                               {
                                   var locker = new object();
                                   decimal latestPrice = 0;

                                   //subscribe to price and update trades with the latest price
                                   var priceHasChanged = marketDataService.Watch(groupedData.Key)
                                       .Synchronize(locker)
                                       .Subscribe(price =>
                                                  {
                                                      latestPrice = price.Bid;
                                                      UpdateTradesWithPrice(groupedData.Cache.Items, latestPrice);
                                                  });
                                  
                                   //connect to data changes and update with the latest price
                                   var dataHasChanged = groupedData.Cache.Connect()
                                       .WhereReasonsAre(ChangeReason.Add, ChangeReason.Update)
                                       .Synchronize(locker)
                                       .Subscribe(changes => UpdateTradesWithPrice(changes.Select(change => change.Current), latestPrice));

                                   return new CompositeDisposable(priceHasChanged, dataHasChanged);

                               })
                .Subscribe();
        }

        private void UpdateTradesWithPrice(IEnumerable<Trade> trades, decimal price)
        {
            trades.ForEach(t=>t.SetMarketPrice(price));
        }

        public void Dispose()
        {
            _job.Dispose();
        }
    }
}
