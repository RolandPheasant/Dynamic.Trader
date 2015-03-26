using System.Windows.Controls.Primitives;

namespace Trader.Client.Infrastucture
{
    public interface IAttachedSelector
    {
        void Receive(Selector selector);
    }
}