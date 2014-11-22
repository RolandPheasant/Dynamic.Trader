
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using TradeExample;

namespace Trader.Console
{
    class Program
    {


        static void Main(string[] args)
        {
             var service = new TradeService();
             //var consoleWriter = service.Trades.Connect()
             //   .Subscribe((IChangeSet<Trade,long> changes) =>
             //              {

             //              });

            System.Console.ReadLine();

        }
    }
}
