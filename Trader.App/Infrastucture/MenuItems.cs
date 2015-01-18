using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Trader.Client.Views;
using Trader.Domain.Infrastucture;

namespace Trader.Client.Infrastucture
{
    public class MenuItems: IDisposable
    {
        private readonly ILogger _logger;
        private readonly IObjectProvider _objectProvider;
        private readonly IEnumerable<MenuItem> _menu;

        private readonly ISubject<ViewContainer> _viewCreatedSubject = new Subject<ViewContainer>();
        private readonly IDisposable _cleanUp;

        public MenuItems(ILogger logger, IObjectProvider objectProvider)
        {
            _logger = logger;
            _objectProvider = objectProvider;

            _menu = new List<MenuItem>
            {
                new MenuItem("Live Trades",
                    "A basic example, illustrating how to connect to a stream, inject a user filter and bind.",
                    () => Open<LiveTradesViewer>("Live Trades"),new []
                        {
                            new Link("Service","TradeService.cs", "https://github.com/RolandPheasant/TradingDemo/blob/master/Trader.Domain/Services/TradeService.cs"), 
                            new Link("View Model","LiveTradesViewer.cs", "https://github.com/RolandPheasant/TradingDemo/blob/master/Trader.App/Views/LiveTradesViewer.cs "), 
                            new Link("Blog","Ui Integration", "https://dynamicdataproject.wordpress.com/2014/11/24/trading-example-part-3-integrate-with-ui/"), 
                        }),



                new MenuItem("Near to Market",
                     "Dynamic filtering of calculated values.",
                     () => Open<NearToMarketViewer>("Near to Market"),new []
                        {
                            new Link("Service","NearToMarketService.cs", "https://github.com/RolandPheasant/TradingDemo/blob/master/Trader.Domain/Services/NearToMarketService.cs"),
                            new Link("Rates Updater","TradePriceUpdateJob.cs", "https://github.com/RolandPheasant/TradingDemo/blob/master/Trader.Domain/Services/TradePriceUpdateJob.cs"), 
                            new Link("View Model", "NearToMarketViewer.cs","https://github.com/RolandPheasant/TradingDemo/blob/master/Trader.App/Views/NearToMarketViewer.cs"), 
                            new Link("Blog i","Manage Market Data", "https://dynamicdataproject.wordpress.com/2014/11/22/trading-example-part-2-manage-market-data/"),
                            new Link("Blog ii","Filter on calculated values", "https://dynamicdataproject.wordpress.com/2014/12/21/trading-example-part-4-filter-on-calculated-values/"),
                            
                        }),

                new MenuItem("Trades By %",  
                       "Group trades by a constantly changing calculated value. With automatic regrouping.",
                        () => Open<TradesByPercentViewer>("Trades By % Diff"),new []
                            {
                                new Link("View Model", "TradesByPercentViewer.cs", "https://github.com/RolandPheasant/TradingDemo/blob/master/Trader.App/Views/TradesByPercentViewer.cs"), 
                                new Link("Group Model","TradesByPercentDiff.cs", "https://github.com/RolandPheasant/TradingDemo/blob/master/Trader.Domain/Model/TradesByPercentDiff.cs"),
                            }),

                new MenuItem("Trades By hh:mm",   
                       "Group items by time with automatic regrouping as time passes",
                        () => Open<TradesByTimeViewer>("Trades By hh:mm"),new []
                        {
                            new Link("View Model","TradesByTimeViewer.cs" ,"https://github.com/RolandPheasant/TradingDemo/blob/master/Trader.App/Views/TradesByTimeViewer.cs"), 
                            new Link("Group Model","TradesByTime.cs", "https://github.com/RolandPheasant/TradingDemo/blob/master/Trader.Domain/Model/TradesByTime.cs"),
                        }),
                
                        new MenuItem("Recent Trades",   
                            "Operator which only includes trades which have changed in the last minute.",
                            () => Open<RecentTradesViewer>("Recent Trades"),new []
                            {
                                new Link("View Model", "RecentTradesViewer.cs","https://github.com/RolandPheasant/TradingDemo/blob/master/Trader.App/Views/RecentTradesViewer.cs"), 
                            }),


                 new MenuItem("Reactive UI",
                    "A basic example, illustrating where reactive-ui and dynamic data can work together",
                    () => Open<RxUiViewer>("Reactive UI Example"),new []
                        {
                             new Link("View Model","RxUiViewer.cs", "https://github.com/RolandPheasant/TradingDemo/blob/master/Trader.App/Views/RxUiViewer.cs "), 
                            new Link("Blog","Ui Integration", "https://dynamicdataproject.wordpress.com/2014/11/24/trading-example-part-3-integrate-with-ui/"), 
                        }),
            };

            _cleanUp = Disposable.Create(() =>
            {
                _viewCreatedSubject.OnCompleted();
            });
        }

        private void Open<T>(string title)
        {
            _logger.Info("Opening '{0}'", title);
            var content = _objectProvider.Get<T>();
            _viewCreatedSubject.OnNext(new ViewContainer(title, content));

            _logger.Info("--Opened '{0}'", title);
        }


        public IObservable<ViewContainer> ItemCreated
        {
            get { return _viewCreatedSubject.AsObservable(); }
        }

        public IEnumerable<MenuItem> Menu
        {
            get { return _menu; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}