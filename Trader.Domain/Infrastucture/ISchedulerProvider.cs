using System.Reactive.Concurrency;

namespace Trader.Domain.Infrastucture
{
    public interface ISchedulerProvider
    {
        IScheduler Dispatcher { get; }
        IScheduler TaskPool { get; }
    }
}
