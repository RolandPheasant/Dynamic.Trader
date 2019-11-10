using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;
using Dragablz;
using DynamicData;
using DynamicData.Binding;
using Trader.Domain.Infrastucture;

namespace Trader.Client.Infrastucture
{
    public class WindowViewModel: AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IObjectProvider _objectProvider;
        private readonly Command _showMenuCommand;
        private readonly IDisposable _cleanUp;
        private ViewContainer _selected;

        public ICommand MemoryCollectCommand { get; } = new Command(() =>
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        });


        public WindowViewModel(IObjectProvider objectProvider, IWindowFactory windowFactory)
        {
            _objectProvider = objectProvider;
            InterTabClient = new InterTabClient(windowFactory);
            _showMenuCommand =  new Command(ShowMenu,()=> Selected!=null && !(Selected.Content is MenuItems));
            ShowInGitHubCommand = new Command(()=>   Process.Start( new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = "/c start https://github.com/RolandPheasant"
            }));

            var menuController = Views.ToObservableChangeSet()
                                        .Filter(vc => vc.Content is MenuItems)
                                        .Transform(vc => (MenuItems) vc.Content)
                                        .MergeMany(menuItem => menuItem.ItemCreated)
                                        .Subscribe(item =>
                                        {
                                            Views.Add(item);
                                            Selected = item;
                                        });
            

            _cleanUp = Disposable.Create(() =>
                                         {
                                             menuController.Dispose();
                                             foreach (var disposable in  Views.Select(vc=>vc.Content).OfType<IDisposable>())
                                                 disposable.Dispose();
                                         });
        }
        
        public void ShowMenu()
        {
            var existing = Views.FirstOrDefault(vc => vc.Content is MenuItems);
            if (existing == null)
            {
                var newmenu = _objectProvider.Get<MenuItems>();
                var newItem = new ViewContainer("Menu", newmenu);
                Views.Add(newItem);
                Selected = newItem;
            }
            else
            {
                Selected = existing;
            }
        }


        public ItemActionCallback ClosingTabItemHandler => ClosingTabItemHandlerImpl;

        private void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
        {
            var container = (ViewContainer)args.DragablzItem.DataContext;//.DataContext;
            if (container.Equals(Selected))
            {
                Selected = Views.FirstOrDefault(vc => vc != container);
            }
            var disposable = container.Content as IDisposable;
            disposable?.Dispose();
        }

        public ObservableCollection<ViewContainer> Views { get; } = new ObservableCollection<ViewContainer>();

        public ViewContainer Selected
        {
            get => _selected;
            set => SetAndRaise(ref _selected, value);
        }

        public IInterTabClient InterTabClient { get; }

        public ICommand ShowMenuCommand => _showMenuCommand;

        public Command ShowInGitHubCommand { get; }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
