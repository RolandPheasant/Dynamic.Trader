using System.Reactive.Concurrency;
using System.Windows.Threading;

namespace Trader.Domain.Infrastucture
{
    public class SchedulerProvider : ISchedulerProvider
    {
        private readonly IScheduler _mainThread;

        public SchedulerProvider(Dispatcher dispatcher)
        {
            _mainThread = new DispatcherScheduler(dispatcher);
        }

        public IScheduler MainThread
        {
            get { return _mainThread; }
        }

        public IScheduler TaskPool
        {
            get { return TaskPoolScheduler.Default; }
        }
    }
}