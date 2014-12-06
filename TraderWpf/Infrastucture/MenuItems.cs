using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TradeExample;
using TradeExample.Infrastucture;

namespace TraderWpf.Infrastucture
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
                new MenuItem("Live Trades",       () => Open<LiveTradesViewer>("Live Trades")),
                new MenuItem("Near to Market",    () => Open<NearToMarketViewer>("Near to Market")),
                new MenuItem("Trades By %",    () => Open<TradesByPercentViewer>("Trades By % Diff")),
                new MenuItem("Recent Trades",    () => Open<RecentTradesViewer>("Recent Trades")),
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