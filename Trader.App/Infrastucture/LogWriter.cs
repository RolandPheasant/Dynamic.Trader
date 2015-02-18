using System;
using Trader.Domain.Infrastucture;

namespace Trader.Client.Infrastucture
{
    public sealed class LogWriter: IDisposable
    {
        private readonly IDisposable _job;

        public LogWriter(ILogEntryService logEntryService)
        {
            _job = ReactiveLogAppender.LogEntryObservable
                .Subscribe(logEntryService.Add);
        }

        public void Dispose()
        {
            _job.Dispose();
        }
    }
}