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
            var view = new TraderWindow();
            var model = _objectProvider.Get<TraderWindowModel>();
            view.DataContext = model;
            return new NewTabHost<Window>(view, view.InitialTabablzControl);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            return TabEmptiedResponse.CloseWindow;
        }
    }
}
