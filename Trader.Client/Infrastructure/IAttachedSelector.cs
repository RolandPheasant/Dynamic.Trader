using System.Windows.Controls.Primitives;

namespace Trader.Client.Infrastructure
{
    public interface IAttachedSelector
    {
        void Receive(Selector selector);
    }
}