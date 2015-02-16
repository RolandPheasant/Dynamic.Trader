using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Trader.Client.Infrastucture
{

    public enum LinkCategory
    {
        Code,
        Blog
    }

    public class MenuItem
    {
        private readonly string _title;
        private readonly string _description;
        private readonly object _content;
        private readonly IEnumerable<Link> _link;
        private readonly ICommand _command;
        private readonly MenuCategory _category;

        public MenuItem(string title, string description, Action action,  
            IEnumerable<Link> link = null,
            object content = null)
        {
            _title = title;
            _description = description;
            _content = content;
            _category = MenuCategory.DynamicData;
            _link = link ?? Enumerable.Empty<Link>();
            _command = new Command(action); ;
        }

        public MenuItem(string title,string description ,Action action, MenuCategory category,
            IEnumerable<Link> link = null,
            object content=null)
        {
            _title = title;
            _description = description;
            _category = category;
            _link = link ?? Enumerable.Empty<Link>();
            _command = new Command(action); ;
        }
        

        public string Title
        {
            get { return _title; }
        }

        public ICommand Command
        {
            get { return _command; }
        }

        public IEnumerable<Link> Link
        {
            get { return _link; }
        }

        public string Description
        {
            get { return _description; }
        }

        public object Content
        {
            get { return _content; }
        }
    }
}