using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using StructureMap;
using Trader.Domain.Services;
using ILogger = Trader.Domain.Infrastucture.ILogger;

namespace Trader.Client.Infrastructure;

internal class AppRegistry : Registry
{
    public AppRegistry()
    {
        //set up logging
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
        if (!File.Exists(path))
            throw new FileNotFoundException("The log4net.config file was not found" + path);

        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        log4net.Config.XmlConfigurator.ConfigureAndWatch(logRepository, new FileInfo(path));
        For<ILogger>().Use<Log4NetLogger>().Ctor<Type>("type").Is(x => x.RootType).AlwaysUnique();

        //
        For<TradePriceUpdateJob>().Singleton();

        Scan(scanner =>
        {
            scanner.ExcludeType<ILogger>();
            scanner.LookForRegistries();
            scanner.Convention<AppConventions>();
            scanner.AssemblyContainingType<AppRegistry>();
            scanner.AssemblyContainingType<TradeService>();
        });
    }
}