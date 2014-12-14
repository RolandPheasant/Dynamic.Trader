using System.Windows;
using Dragablz;

namespace Trader.Client
{
    public class InterTabClient : IInterTabClient
    {
        private readonly TraderWindowFactory _factory;

        public InterTabClient( TraderWindowFactory tradeWindowFactory)
        {
            _factory = tradeWindowFactory;
        }

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var window = _factory.Create();

            return new NewTabHost<Window>(window, window.InitialTabablzControl);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            return TabEmptiedResponse.CloseWindow;
        }
    }
}
