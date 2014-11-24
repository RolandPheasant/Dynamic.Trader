using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using TradeExample;
using TradeExample.Infrastucture;
using TraderWpf.Infrastucture;

namespace TraderWpf
{
    internal class AppRegistry : Registry
    {
        public AppRegistry()
        {
            // Because we're using Log4Net .. which requires some configuration information ... lets load that now.
            // This trick with AppDomain was stolen from NLog :)
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            if (!File.Exists(path))
                throw new FileNotFoundException("The log4net.config file was not found" + path);

            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(path));

            For<ILogger>().Use<Log4NetLogger>().Ctor<Type>("type").Is(x => x.RootType);
         
            For<IObjectProvider>().Singleton().Use<ObjectProvider>();
            For<ITradeService>().Singleton().Use<TradeService>();
            For<IStaticData>().Singleton().Use<StaticData>();
            For<IMarketPriceService>().Singleton().Use<MarketPriceService>();
            For<INearToMarketService>().Singleton().Use<NearToMarketService>();
            

            ForConcreteType<ViewsCollection>().Configure.Singleton();
            For<UnhandledExceptionEventHandler>().Singleton();
            For<TradePriceUpdateJob>().Singleton();

            Scan(scanner => scanner.LookForRegistries());
        }
    }
}

