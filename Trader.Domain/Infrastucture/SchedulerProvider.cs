using System;
using System.Reactive.Concurrency;
using System.Windows.Threading;

namespace Trader.Domain.Infrastucture
{
    public class MyDispatcherScheduler: IScheduler
    {
        private readonly DispatcherScheduler _dispatcherScheduler;
        private readonly ImmediateScheduler _immediateScheduler = ImmediateScheduler.Instance;

        public MyDispatcherScheduler(DispatcherScheduler original)
        {
            _dispatcherScheduler = original;
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
           return GetScheduler().Schedule(state, action);
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            return GetScheduler().Schedule(state,dueTime, action);
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            return GetScheduler().Schedule(state, dueTime, action);
        }

        private IScheduler GetScheduler()
        {
            return _dispatcherScheduler.Dispatcher.CheckAccess() 
                ? (IScheduler)_immediateScheduler 
                : (IScheduler)_dispatcherScheduler;
        }

        public DateTimeOffset Now { get { return _dispatcherScheduler.Now; } }
    }


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