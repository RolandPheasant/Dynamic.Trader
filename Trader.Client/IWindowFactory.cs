namespace Trader.Client
{
    public interface IWindowFactory
    {
        MainWindow Create(bool showMenu=false);
    }
}