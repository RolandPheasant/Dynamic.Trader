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

          //  var boundViewModel = container.GetInstance<ViewsCollection>();

            //run start up jobs
            var priveUpdater = container.GetInstance<TradePriceUpdateJob>();

            var windowModel = container.GetInstance<TraderWindowModel>();
           // var items = container.GetInstance<MenuItems>();
          //  boundViewModel.Items.Add(new ViewContainer("Menu", items));
            new TraderWindow()
            {
                DataContext = windowModel
            }.Show();

            app.Resources.Add(SystemParameters.ClientAreaAnimationKey, null);
            app.Resources.Add(SystemParameters.MinimizeAnimationKey, null);
            app.Resources.Add(SystemParameters.UIEffectsKey, null);


            app.Run();
        }
    }
}
