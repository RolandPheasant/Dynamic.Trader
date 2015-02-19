using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;
using Dragablz;
using DynamicData;
using DynamicData.Binding;
using Trader.Client.Infrastucture;
using Trader.Domain.Infrastucture;

namespace Trader.Client
{
    public class WindowViewModel: AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IInterTabClient _interTabClient;
        private readonly IObjectProvider _objectProvider;
        private readonly Command _showInGitHubCommand;
        private readonly ICommand _showMenuCommand;
        private readonly IDisposable _cleanUp;
        private readonly ObservableCollection<ViewContainer> _data = new ObservableCollection<ViewContainer>();
        private ViewContainer _selected;

        public WindowViewModel(IObjectProvider objectProvider, IWindowFactory windowFactory)
        {
            _objectProvider = objectProvider;
            _interTabClient = new InterTabClient(windowFactory);
            _showMenuCommand =  new Command(ShowMenu);
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
                                         });
        }
        
        public void ShowMenu()
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


        public ClosingTabItemCallback ClosingTabItemHandler
        {
            get { return ClosingTabItemHandlerImpl; }
        }

        private  void ClosingTabItemHandlerImpl(ClosingItemCallbackArgs<TabablzControl> args)
        {
            var container = (ViewContainer)args.DragablzItem.DataContext;
            if (container.Equals(Selected)) Selected = _data.FirstOrDefault(vc => vc != container);
            var disposable = container.Content as IDisposable;
            if (disposable != null) disposable.Dispose();
        }

        public ObservableCollection<ViewContainer> Views
        {
            get { return _data; }
        }

        public ViewContainer Selected
        {
            get { return _selected; }
            set { SetAndRaise(ref _selected, value); }
        }

        public IInterTabClient InterTabClient
        {
            get { return _interTabClient; }
        }

        public ICommand ShowMenuCommand
        {
            get { return _showMenuCommand; }
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
