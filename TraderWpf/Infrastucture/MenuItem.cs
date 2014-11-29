using System;
using System.Windows.Input;

namespace TraderWpf
{
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