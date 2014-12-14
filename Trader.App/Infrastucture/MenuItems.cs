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
                new MenuItem("Live Trades",       () => Open<LiveTradesViewer>("Live Trades"),new []
                                                                                              {
                                                                                                  new Link("View Model","LiveTradesViewer.cs", "https://github.com/RolandPheasant/TradingDemo/blob/master/TradeExample/LiveTradesViewer.cs"), 
                                                                                                  new Link("Service","TradeService.cs", "https://github.com/RolandPheasant/TradingDemo/blob/master/TradeExample/TradeService.cs"), 
                                                                                                  new Link("Blog","Ui Integration", "https://dynamicdataproject.wordpress.com/2014/11/24/trading-example-part-3-integrate-with-ui/"), 
                                                                                              }),
                new MenuItem("Near to Market",    () => Open<NearToMarketViewer>("Near to Market"),new []
                                                                                              {
                                                                                                  new Link("View Model", "https://github.com/RolandPheasant/TradingDemo/blob/master/TradeExample/LiveTradesViewer.cs"), 
                                                                                                  new Link("Trade Service", "https://github.com/RolandPheasant/TradingDemo/blob/master/TradeExample/TradeService.cs"), 
                                                                                              }),
                new MenuItem("Trades By %",    () => Open<TradesByPercentViewer>("Trades By % Diff"),new []
                                                                                              {
                                                                                                  new Link("View Model", "https://github.com/RolandPheasant/TradingDemo/blob/master/TradeExample/LiveTradesViewer.cs"), 
                                                                                                  new Link("Trade Service", "https://github.com/RolandPheasant/TradingDemo/blob/master/TradeExample/TradeService.cs"), 
                                                                                              }),
                new MenuItem("Trades By hh:mm",    () => Open<TradesByTimeViewer>("Trades By hh:mm"),new []
                                                                                              {
                                                                                                  new Link("View Model", "https://github.com/RolandPheasant/TradingDemo/blob/master/TradeExample/LiveTradesViewer.cs"), 
                                                                                                  new Link("Trade Service", "https://github.com/RolandPheasant/TradingDemo/blob/master/TradeExample/TradeService.cs"), 
                                                                                              }),
                new MenuItem("Recent Trades",    () => Open<RecentTradesViewer>("Recent Trades"),new []
                                                                                              {
                                                                                                  new Link("View Model", "https://github.com/RolandPheasant/TradingDemo/blob/master/TradeExample/LiveTradesViewer.cs"), 
                                                                                                  new Link("Trade Service", "https://github.com/RolandPheasant/TradingDemo/blob/master/TradeExample/TradeService.cs"), 
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