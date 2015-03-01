
using ReactiveUI;

namespace Trader.Client.Views
{
    public class RxUiHostViewModel
    {
        private readonly ReactiveObject _content;

        public RxUiHostViewModel(ReactiveObject content)
        {
            _content = content;
        }

        public ReactiveObject Content
        {
            get { return _content; }
        }
    }
}
