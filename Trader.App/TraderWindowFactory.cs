using System;
using Dragablz;
using Trader.Domain.Infrastucture;
using TraderWpf;

namespace Trader.Client
{
    public class TraderWindowFactory
    {
        private readonly IObjectProvider _objectProvider;

        public TraderWindowFactory(IObjectProvider objectProvider)
        {
            _objectProvider = objectProvider;
        }

        public TraderWindow Create(bool showMenu=false)
        {
            var window = new TraderWindow();
            var model = _objectProvider.Get<TraderWindowModel>();
            if (showMenu) model.OnShowMenu();

            window.DataContext = model;

            window.Closing += (sender, e) =>
                              {
                                  if (TabablzControl.GetIsClosingAsPartOfDragOperation(window)) return;

                                  var todispose = ((TraderWindow) sender).DataContext as IDisposable;
                                  if (todispose != null) todispose.Dispose();
                              };

            return window;
        }
    }
}