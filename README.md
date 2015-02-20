## Dynamic Data Demo

This is a c# sample wpf project to help get started with Dynamic Data.  I have tried to make the code behind each screen as simple as posible so it is easy to follow.

Source code for dynamic data is at [Dynamic Data on GitHub](https://github.com/RolandPheasant/DynamicData) and the blog at http://dynamic-data.org/

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
 
![Alt text](https://github.com/RolandPheasant/TradingDemo/blob/master/Images/LiveTrades.gif "Sample Screen Shot")

or how the following extract

 ```csharp  
 _cleanUp = tradeService.Live.Connect()
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

![Alt text](https://github.com/RolandPheasant/TradingDemo/blob/master/Images/Positions.gif "Positions View")

This is so easy, a few lines of code which after a short learning curve becomes very easy.

Plus many more dynamic data examples. Additionally there are some examples which show how to integrate with [ReactiveUI](https://github.com/reactiveui/ReactiveUI). 

The menu looks like this and as you can see there are links to the code behind which hopefully will get you up to speed in no time at all

![Alt text](https://github.com/RolandPheasant/TradingDemo/blob/master/Images/Menu.gif "Menu with links")

The examples are being regularly maintained so download again to see more examples.

## Run the demo

- Press 'Download Zip', unpack and open
- Ensure Nuget Restore is enabled
- Set 'Trader.Client' as the startup project
- Press F5

## Feedback

[![Join the chat at https://gitter.im/RolandPheasant/TradingDemo](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/RolandPheasant/TradingDemo?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
 - Follow me at [@RolandPheasant](https://twitter.com/RolandPheasant) 
 - Email at [roland@dynamic-data.org]
  
Also a big thanks to the guys behind [Mahapps](https://github.com/MahApps/MahApps.Metro) and [Dragablz](https://github.com/ButchersBoy/Dragablz) for their awesome oss projects. These have enabled me to easily make this demo look good. 








