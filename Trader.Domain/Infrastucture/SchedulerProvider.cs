using System.Reactive.Concurrency;
using System.Windows.Threading;

namespace Trader.Domain.Infrastucture
{
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