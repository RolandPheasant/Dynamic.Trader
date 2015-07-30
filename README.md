## Dynamic Trader

Dynamic data is a portable class library which brings the power of reactive (rx) to collections.  It is open source and the code base lives here [Dynamic Data on GitHub](https://github.com/RolandPheasant/DynamicData). 

To help illustrate how powerful  and easy Dynamic Data is, and to help you get up and running I have created this WPF demo app.  I have made the code behind each screen as simple as possible so it is easy to follow.  

Most of the example screens have been written about on the blog at [dynamic-data.org](http://dynamic-data.org/)

The demo illustrates how the following code:

```csharp
var loader = tradeService.All
    .Connect(trade => trade.Status == TradeStatus.Live) //prefilter live trades only
    .Filter(_filter) // apply user filter
    .Transform(trade => new TradeProxy(trade),new ParallelisationOptions(ParallelType.Ordered,5))
    .Sort(SortExpressionComparer<TradeProxy>.Descending(t => t.Timestamp),SortOptimisations.ComparesImmutableValuesOnly)
    .ObserveOnDispatcher()
    .Bind(_data)   // update observable collection bindings
    .DisposeMany() //since TradeProxy is disposable dispose when no longer required
    .Subscribe();
``` 
 produces this
 
![Live Trades View](https://github.com/RolandPheasant/TradingDemo/blob/master/Images/LiveTrades.gif "Live Trades View")

or how the following extract

 ```csharp  
 var loader = tradeService.Live.Connect()
            .Group(trade => trade.CurrencyPair)
            .Transform(group => new CurrencyPairPosition(group))
            .Sort(SortExpressionComparer<CurrencyPairPosition>.Ascending(t => t.CurrencyPair))
            .ObserveOn(schedulerProvider.MainThread)
            .Bind(_data)
            .DisposeMany()
            .Subscribe();

	//when CurrencyPairPosition class does this
	tradesByCurrencyPair.Cache.Connect()
			.QueryWhenChanged(query =>
			{
				var buy = query.Items.Where(trade => trade.BuyOrSell == BuyOrSell.Buy).Sum(trade=>trade.Amount);
				var sell = query.Items.Where(trade => trade.BuyOrSell == BuyOrSell.Sell).Sum(trade => trade.Amount);
				var count = query.Count;
				return new TradesPosition(buy,sell,count);
			})
			.Subscribe(position => Position = position);
        }
```
Produces this.

![Aggregated Positions View](https://github.com/RolandPheasant/TradingDemo/blob/master/Images/Positions.gif "Aggregated Positions View")

This is so easy, a few lines of code which after a short learning curve becomes very easy.

Plus many more dynamic data examples. Additionally there are some examples which show how to integrate with [ReactiveUI](https://github.com/reactiveui/ReactiveUI). 

The menu looks like this and as you can see there are links to the code behind which hopefully will get you up to speed in no time at all

![Menu](https://github.com/RolandPheasant/TradingDemo/blob/master/Images/Menu.gif "Menu with links")

The examples are being regularly maintained so download again to see more examples.

## Run the demo

- [![Build status](https://ci.appveyor.com/api/projects/status/axcp2ktriyix9blt/branch/master?svg=true)](https://ci.appveyor.com/project/RolandPheasant/tradingdemo/branch/master) 
- Press 'Download Zip', unpack and open Dynamic.Trader.sln
- Ensure Nuget Restore is enabled
- Set 'Trader.Client' as the startup project
- Press F5

## Feedback

Please feel free to me a on any of the following channels
- [![Join the chat at https://gitter.im/RolandPheasant/DynamicData](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/RolandPheasant/DynamicData?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
 - Follow me on twitter [@RolandPheasant](https://twitter.com/RolandPheasant) 
 - Email at [roland@dynamic-data.org](roland@dynamic-data.org)
  
Also a big thanks to the guys behind [Mahapps](https://github.com/MahApps/MahApps.Metro) and [Dragablz](https://github.com/ButchersBoy/Dragablz) for their awesome oss projects. These have enabled me to easily make this demo look good. 

