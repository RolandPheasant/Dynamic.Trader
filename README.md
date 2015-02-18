## Dynamic Data Demo

This is a c# sample wpf project to help get started with Dynamic Data.  Source code for dynamic data is at https://github.com/RolandPheasant/DynamicData and the blog at http://dynamic-data.org/

A big thanks to these guys for their awesome oss projects

- Mahaps https://github.com/MahApps/MahApps.Metro
- Dragablz https://github.com/ButchersBoy/Dragablz

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

Plus many more dynamic data examples. Also there are some examples which show how to integrate with [ReactiveUI](https://github.com/reactiveui/ReactiveUI)

The menu looks like this and as you can see there are links to the code behind which hopefully will get you up to speed in no time at all

![Alt text](https://github.com/RolandPheasant/TradingDemo/blob/master/Images/Menu.gif "Menu with links")


The examples will be regularly maintained so download again to see more examples.

## Run the demo

- Press 'Download Zip', unpack and open
- Ensure Nuget Restore is enabled
- Set 'Trader.Client' as the startup project
- Press F5

## Feedback

[![Join the chat at https://gitter.im/RolandPheasant/TradingDemo](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/RolandPheasant/TradingDemo?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Blog at  http://dynamic-data.org/

You can contact me on twitter  [@RolandPheasant](https://twitter.com/RolandPheasant) or email at [roland@dynamic-data.org]







