using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragablz;
using TradeExample.Infrastucture;

namespace TraderWpf
{
    public class TraderWindowModel: AbstractNotifyPropertyChanged
    {
        private readonly IInterTabClient _interTabClient;
        private readonly MenuItems _menu;
        private readonly ViewsCollection _views;
        private ViewContainer _selected;


        public TraderWindowModel(MenuItems menu, ViewsCollection views,IObjectProvider objectProvider )
        {
            _menu = menu;
            _views = views;
            _interTabClient = new InterTabClient(objectProvider);

            var opener = Menu.ItemCreated.Subscribe(item =>
                                                    {
                                                        Views.Add(item);
                                                        Selected = item;
                                                    });

        }

        public MenuItems Menu
        {
            get { return _menu; }
        }

        public ViewsCollection Views
        {
            get { return _views; }
        }

        public ViewContainer Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }

        public IInterTabClient InterTabClient
        {
            get { return _interTabClient; }
        }
    }
}
