using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Dragablz;
using DynamicData;
using TradeExample.Infrastucture;

namespace TraderWpf
{
    public class TraderWindowModel: AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IInterTabClient _interTabClient;
        private readonly IObjectProvider _objectProvider;
        private readonly ObservableCollection<ViewContainer> _data = new ObservableCollection<ViewContainer>();
        private readonly ICommand _showMenu;
        private readonly IDisposable _cleanUp;
        private readonly SerialDisposable _newMenuItemSubscriber = new SerialDisposable();

        private ViewContainer _selected;

        public TraderWindowModel(IObjectProvider objectProvider,TraderWindowFactory traderWindowFactory )
        {
            _objectProvider = objectProvider;
            _interTabClient = new InterTabClient(traderWindowFactory);
            _showMenu =  new Command(OnShowMenu);


            var menuController = _data.WatchChanges(vc => vc.Id)
                .Filter(vc => vc.Content is MenuItems)
                .Transform(vc => (MenuItems) vc.Content)
                .SubscribeMany(menuItem => menuItem.ItemCreated.Subscribe(item =>
                                                                          {
                                                                              _data.Add(item);
                                                                              Selected = item;
                                                                          }))
                .Subscribe();

            _cleanUp = Disposable.Create(() =>
                                         {

                                             menuController.Dispose();
                                             foreach (var disposable in  _data.Select(vc=>vc.Content).OfType<IDisposable>())
                                                 disposable.Dispose();
                                             
                                             _newMenuItemSubscriber.Dispose();
                                         });
        }

        public void OnShowMenu()
        {
            var existing = _data.FirstOrDefault(vc => vc.Content is MenuItems);
            if (existing == null)
            {
                var newmenu = _objectProvider.Get<MenuItems>();
                var newItem = new ViewContainer("Menu", newmenu);
                _data.Add(newItem);
                Selected = newItem;

                //_newMenuItemSubscriber.Disposable = newmenu.ItemCreated.Subscribe(item =>
                //{
                //    _data.Add(item);
                //    Selected = item;
                //});
            }
            else
            {
                Selected = existing;
            }
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

        public ICommand ShowMenu
        {
            get { return _showMenu; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
