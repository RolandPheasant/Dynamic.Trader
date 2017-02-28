using System;
using System.Reactive.Concurrency;
using System.Windows.Threading;

namespace Trader.Domain.Infrastucture
{
    public class SchedulerProvider : ISchedulerProvider
    {
        public SchedulerProvider(Dispatcher dispatcher)
        {
            MainThread = new DispatcherScheduler(dispatcher);
        }

        public IScheduler MainThread { get; }
        public IScheduler Background => TaskPoolScheduler.Default;
    }
}