using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;

namespace TradeExample
{
    public class TradePriceUpdateJob
    {
        private IDisposable _job;

        public TradePriceUpdateJob(ITradeService tradeService)
        {
            _job = tradeService.Trades.Connect()
                .Filter(trade => trade.Status == TradeStatus.Live)
                .Group(trade => trade.CurrencyPair)
                .SubscribeMany(groupedData =>
                               {
                                   var locker = new object();
                                   decimal latestPrice = 0;

                                   //subscribe to price and update trades with the latest price
                                   var priceHasChanged = MarketPriceService.ObservePrice(groupedData.Key)
                                       .Synchronize(locker)
                                       .Subscribe(price =>
                                                  {
                                                      latestPrice = price;
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
            foreach (var trade in trades)
            {
                trade.Price = price;
            }
        }
    }
}
