using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;
using Dragablz;
using Dragablz.Dockablz;
using DynamicData;
using DynamicData.Binding;
using MahApps.Metro.Actions;
using Trader.Domain.Infrastucture;

namespace Trader.Client.Infrastucture
{
    public class TraderWindowModel: AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IInterTabClient _interTabClient;
        private readonly IObjectProvider _objectProvider;
        private readonly Command _showInGitHubCommand;
        private readonly ICommand _showMenu;
        private readonly IDisposable _cleanUp;
        private readonly SerialDisposable _newMenuItemSubscriber = new SerialDisposable();
        private readonly ObservableCollection<ViewContainer> _data = new ObservableCollection<ViewContainer>();

        private ViewContainer _selected;

        public TraderWindowModel(IObjectProvider objectProvider,TraderWindowFactory traderWindowFactory )
        {
            _objectProvider = objectProvider;
            _interTabClient = new InterTabClient(traderWindowFactory);
            _showMenu =  new Command(OnShowMenu);
            _showInGitHubCommand = new Command(()=>   Process.Start("https://github.com/RolandPheasant"));

            var menuController = _data.ToObservableChangeSet(vc => vc.Id)
                                        .Filter(vc => vc.Content is MenuItems)
                                        .Transform(vc => (MenuItems) vc.Content)
                                        .MergeMany(menuItem => menuItem.ItemCreated)
                                        .Subscribe(item =>
                                        {
                                            _data.Add(item);
                                            Selected = item;
                                        });



            _cleanUp = Disposable.Create(() =>
                                         {
                                             menuController.Dispose();
                                             foreach (var disposable in  _data.Select(vc=>vc.Content).OfType<IDisposable>())
                                                 disposable.Dispose();
                                             
                                             _newMenuItemSubscriber.Dispose();
                                         });
        }


        public ClosingTabItemCallback ClosingTabItemHandler
        {
            get { return ClosingTabItemHandlerImpl; }
        }

        private static void ClosingTabItemHandlerImpl(ClosingItemCallbackArgs<TabablzControl> args)
        {
            var container = (ViewContainer)args.DragablzItem.DataContext;
           var disposable = container.Content as IDisposable;
            if (disposable!=null) disposable.Dispose();
            //here's how you can cancel stuff:
            //args.Cancel(); 
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

        public Command ShowInGitHubCommand
        {
            get { return _showInGitHubCommand; }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
