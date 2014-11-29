using System;
using System.Windows;
using Dragablz;
using TradeExample.Infrastucture;

namespace TraderWpf
{

    public class InterTabClient : IInterTabClient
    {
        private readonly IObjectProvider _objectProvider;

        public InterTabClient(IObjectProvider objectProvider)
        {
            _objectProvider = objectProvider;
        }

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var window = new TraderWindow();
            var model = _objectProvider.Get<TraderWindowModel>();
            window.DataContext = model;

            window.Closing += (sender, e) =>
                             {
                                 var todispose = ((TraderWindow) sender).DataContext as IDisposable;
                                 if (todispose!=null) todispose.Dispose();
                             };

            return new NewTabHost<Window>(window, window.InitialTabablzControl);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            return TabEmptiedResponse.CloseWindow;
        }
    }
}
