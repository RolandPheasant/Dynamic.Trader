namespace Trader.Client.Infrastructure
{
    public interface IWindowFactory
    {
        MainWindow Create(bool showMenu=false);
    }
}