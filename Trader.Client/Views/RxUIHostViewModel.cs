
using ReactiveUI;

namespace Trader.Client.Views
{
    public class RxUiHostViewModel
    {
        public RxUiHostViewModel(ReactiveObject content)
        {
            Content = content;
        }

        public ReactiveObject Content { get; }
    }
}
