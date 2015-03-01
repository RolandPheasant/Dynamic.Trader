using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Trader.Client.Views
{
    public class ReactiveUIHostViewModel
    {
        private readonly ReactiveObject _content;

        public ReactiveUIHostViewModel(ReactiveObject content)
        {
            _content = content;
        }

        public ReactiveObject Content
        {
            get { return _content; }
        }
    }
}
