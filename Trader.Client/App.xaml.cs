using System;
using System.Windows;
using System.Windows.Threading;
using ReactiveUI;
using Splat;
using StructureMap;
using Trader.Client.Infrastucture;
using Trader.Client.Views;
using Trader.Domain.Infrastucture;
using Trader.Domain.Services;

namespace Trader.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Application" /> class.</summary>
        /// <exception cref="T:System.InvalidOperationException">More than one instance of the <see cref="T:System.Windows.Application" /> class is created per <see cref="T:System.AppDomain" />.</exception>
        public App()
        {
            ShutdownMode = ShutdownMode.OnLastWindowClose;
        }

        /// <summary>Raises the <see cref="E:System.Windows.Application.Startup" /> event.</summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var container = new Container(x => x.AddRegistry<AppRegistry>());
            var factory = container.GetInstance<WindowFactory>();
            var window = factory.Create(true);
            container.Configure(x => x.For<Dispatcher>().Add(window.Dispatcher));

            //configure dependency resolver for RxUI / Splat
            var resolver = new ReactiveUIDependencyResolver(container);
            resolver.Register(() => new LogEntryView(), typeof(IViewFor<LogEntryViewer>));
            resolver.Register(() => new RxUiView(), typeof(IViewFor<RxUiViewer>));
            Locator.Current = resolver;
            RxApp.SupportsRangeNotifications = false;
            //run start up jobs
            container.GetInstance<TradePriceUpdateJob>();
            container.GetInstance<ILogEntryService>();

            window.Show();
            base.OnStartup(e);
        }
    }
}
