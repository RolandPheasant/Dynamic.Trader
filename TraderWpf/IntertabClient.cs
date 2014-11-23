using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dragablz;
using MahApps.Metro.Controls;

namespace TraderWpf
{
    public class InterTabClient : IInterTabClient
    {

        public InterTabClient()
        {
        }

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var view = new TraderWindow();
            var model = new object();
            view.DataContext = model;
            return new NewTabHost<Window>(view, view.InitialTabablzControl);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            return TabEmptiedResponse.CloseWindow;
        }
    }
}
