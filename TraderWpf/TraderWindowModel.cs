using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;
using Dragablz;
using TradeExample.Infrastucture;

namespace TraderWpf
{
    public class TraderWindowModel: AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IInterTabClient _interTabClient;
        private readonly IObjectProvider _objectProvider;
        private ViewContainer _selected;
        private readonly ObservableCollection<ViewContainer> _data = new ObservableCollection<ViewContainer>();
        private readonly ICommand _showMenu;

        private readonly IDisposable _cleanUp;
        private readonly SerialDisposable _menuDisposer = new SerialDisposable();

        public TraderWindowModel(IObjectProvider objectProvider )
        {
            _objectProvider = objectProvider;
            _interTabClient = new InterTabClient(objectProvider);
            _showMenu =  new Command(OnShowMenu);


            _cleanUp = Disposable.Create(() =>
                                         {
                                             foreach (var disposable in  _data.OfType<IDisposable>())
                                                 disposable.Dispose();


                                             _menuDisposer.Dispose();
                                            
                                         });
        }

        private void OnShowMenu()
        {
            var existing = _data.FirstOrDefault(vc => vc.Content is MenuItems);
            if (existing == null)
            {
                var newmenu = _objectProvider.Get<MenuItems>();
                var newItem = new ViewContainer("Menu", newmenu);
                _data.Add(newItem);
                Selected = newItem;

                _menuDisposer.Disposable = newmenu.ItemCreated.Subscribe(item =>
                {
                    _data.Add(item);
                    Selected = item;
                });
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
