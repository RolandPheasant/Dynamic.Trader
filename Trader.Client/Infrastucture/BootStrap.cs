using System;
using System.Windows;
using System.Windows.Threading;
using ReactiveUI;
using Splat;
using StructureMap;
using Trader.Client.Views;
using Trader.Domain.Services;

namespace Trader.Client.Infrastucture
{
    public class BootStrap
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new App { ShutdownMode = ShutdownMode.OnLastWindowClose };
            app.InitializeComponent();


           var container =  new Container(x=> x.AddRegistry<AppRegistry>());
           var factory = container.GetInstance<WindowFactory>();
           var window = factory.Create(true);
           container.Configure(x => x.For<Dispatcher>().Add(window.Dispatcher));

            //configure dependency resolver for RxUI / Splat
            var resolver =  new ReactiveUIDependencyResolver(container);
            resolver.Register(() => new LogEntryView(), typeof(IViewFor<LogEntryViewer>));
            Locator.Current = resolver;

            //run start up jobs
           container.GetInstance<TradePriceUpdateJob>();

            window.Show();
            app.Run();
        }
    }
}
