namespace Trader.Client.Infrastucture
{
    public interface IWindowFactory
    {
        MainWindow Create(bool showMenu=false);
    }
}