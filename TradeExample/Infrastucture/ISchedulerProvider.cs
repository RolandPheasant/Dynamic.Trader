using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TradeExample.Infrastucture
{
    public interface ISchedulerProvider
    {
        IScheduler Dispatcher { get; }
        IScheduler TaskPool { get; }
    }

   public class SchedulerProvider : ISchedulerProvider
    {
       private readonly IScheduler _dispatcher;

       public SchedulerProvider(Dispatcher dispatcher)
       {
           _dispatcher = new DispatcherScheduler(dispatcher);
       }

       public IScheduler Dispatcher
       {
           get { return _dispatcher; }
       }

       public IScheduler TaskPool
       {
           get { return TaskPoolScheduler.Default; }
       }
    }
}
