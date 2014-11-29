using System;
using System.Collections.Generic;
using System.Windows.Input;
using TradeExample;
using TradeExample.Infrastucture;

namespace TraderWpf
{

    public class MenuItems
    {
        private readonly ILogger _logger;
        private readonly IObjectProvider _objectProvider;
        private readonly ViewsCollection _viewsCollection;
        private readonly IEnumerable<MenuItem> _menu;

        public MenuItems(ILogger logger, IObjectProvider objectProvider,ViewsCollection viewsCollection)
        {
            _logger = logger;
            _objectProvider = objectProvider;
            _viewsCollection = viewsCollection;
            _menu = new List<MenuItem>
                {
                    new MenuItem("Live Trades",       () => Open<LiveTradesViewer>("Live Trades")),
                    new MenuItem("Near to Market",    () => Open<NearToMarketViewer>("Near to Market")),
                };
        }

        private void Open<T>(string title)
        {
            _logger.Info("Opening '{0}'", title);
            var content = _objectProvider.Get<T>();
            _viewsCollection.Add(new ViewContainer(title, content));
            _logger.Info("--Opened '{0}'", title);
        }


        public IEnumerable<MenuItem> Menu
        {
            get { return _menu; }
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