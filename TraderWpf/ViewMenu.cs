using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using TradeExample;
using TradeExample.Infrastucture;

namespace TraderWpf
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

    public class MenuItem
    {
        private readonly string _title;
        private readonly ICommand _command;
        private readonly object _tileContent;
        private readonly string _group;


        public MenuItem(string title, Action action, object tileContent = null)
        {
            _title = title;
            _command = new Command(action); ;
            _tileContent = tileContent;
            _group = string.Empty;
        }


        public MenuItem(string title, string group, Action action, object tileContent = null)
        {
            _title = title;
            _command = new Command(action); ;
            _tileContent = tileContent;
            _group = group;
        }

        public string Title
        {
            get { return _title; }
        }

        public ICommand Command
        {
            get { return _command; }
        }

        public object TileContent
        {
            get { return _tileContent; }
        }

        public string Group
        {
            get { return _group; }
        }
    }
}