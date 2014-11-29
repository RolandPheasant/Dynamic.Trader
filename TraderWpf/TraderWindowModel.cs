using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragablz;
using TradeExample.Infrastucture;

namespace TraderWpf
{
    public class TraderWindowModel
    {
        private readonly IInterTabClient _interTabClient;
        private readonly MenuItems _menu;
        private readonly ViewsCollection _views;

        public TraderWindowModel(MenuItems menu, ViewsCollection views,IObjectProvider objectProvider )
        {
            _menu = menu;
            _views = views;
            _interTabClient = new InterTabClient(objectProvider);
        }

        public MenuItems Menu
        {
            get { return _menu; }
        }

        public ViewsCollection Views
        {
            get { return _views; }
        }

        public IInterTabClient InterTabClient
        {
            get { return _interTabClient; }
        }
    }
}
