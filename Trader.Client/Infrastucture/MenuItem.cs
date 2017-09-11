using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Trader.Client.Infrastucture
{
    public class MenuItem
    {
        public MenuItem(string title, string description, Action action,  
            IEnumerable<Link> link = null,
            object content = null)
        {
            Title = title;
            Description = description;
            Content = content;
            Category = MenuCategory.DynamicData;
            Link = link ?? Enumerable.Empty<Link>();
            Command = new Command(action);
        }

        public MenuItem(string title,string description ,Action action, MenuCategory category,
            IEnumerable<Link> link = null,
            object content=null)
        {
            Title = title;
            Description = description;
            Category = category;
            Link = link ?? Enumerable.Empty<Link>();
            Command = new Command(action);
        }
        

        public string Title { get; }

        public ICommand Command { get; }

        public IEnumerable<Link> Link { get; }

        public string Description { get; }

        public object Content { get; }

        public MenuCategory Category { get; }
    }
}