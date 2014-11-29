using System;
using System.Windows;
using StructureMap;
using TradeExample;

namespace TraderWpf
{
    public class BootStrap
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new App { ShutdownMode = ShutdownMode.OnLastWindowClose };
            app.InitializeComponent();

           var container =  new Container(x=> x.AddRegistry<AppRegistry>());

            //run start up jobs
            var priceUpdater = container.GetInstance<TradePriceUpdateJob>();
            var factory = container.GetInstance<TraderWindowFactory>();
            factory.Create(true).Show();

            app.Resources.Add(SystemParameters.ClientAreaAnimationKey, null);
            app.Resources.Add(SystemParameters.MinimizeAnimationKey, null);
            app.Resources.Add(SystemParameters.UIEffectsKey, null);


            app.Run();
        }
    }
}
