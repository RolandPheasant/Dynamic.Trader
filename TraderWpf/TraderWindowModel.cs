using System;
using System.Collections.ObjectModel;
using Dragablz;
using TradeExample.Infrastucture;

namespace TraderWpf
{
    public class TraderWindowModel: AbstractNotifyPropertyChanged
    {
        private readonly IInterTabClient _interTabClient;
        private readonly MenuItems _menu;
        private ViewContainer _selected;
        private readonly ObservableCollection<ViewContainer> _data = new ObservableCollection<ViewContainer>();

        public TraderWindowModel(MenuItems menu, IObjectProvider objectProvider )
        {
            _menu = menu;
            _interTabClient = new InterTabClient(objectProvider);

            var opener = Menu.ItemCreated.Subscribe(item =>
                                                    {
                                                        _data.Add(item);
                                                        Selected = item;
                                                    });


            _data.CollectionChanged += Items_CollectionChanged;

        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        public MenuItems Menu
        {
            get { return _menu; }
        }

        public ObservableCollection<ViewContainer> Views
        {
            get { return _data; }
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
