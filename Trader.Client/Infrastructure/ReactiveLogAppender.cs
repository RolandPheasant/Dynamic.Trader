using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using log4net.Appender;
using log4net.Core;
using Trader.Domain.Infrastucture;

namespace Trader.Client.Infrastructure
{
    public class ReactiveLogAppender : AppenderSkeleton
    {
        private readonly static ISubject<LogEntry> Subject = new Subject<LogEntry>();
        private long _counter;

        public static IObservable<LogEntry> LogEntryObservable
        {
            get { return Subject.AsObservable(); }
        }

        #region Overrides of AppenderSkeleton
        
        protected override void OnClose()
        {
            Subject.OnCompleted();
            base.OnClose();
        }
        
        protected override void Append(LoggingEvent loggingEvent)
        {
            Interlocked.Increment(ref _counter);

            var entry = new LogEntry(_counter,
                loggingEvent.RenderedMessage,
                loggingEvent.LoggerName,
                loggingEvent.ThreadName,
                loggingEvent.TimeStamp,
                Map(loggingEvent.Level),
                loggingEvent.ExceptionObject);

            Subject.OnNext(entry);
        }

        private LogLevel Map(Level level)
        {
            if (level == Level.Info)
                return LogLevel.Info;

            if (level == Level.Debug)
                return LogLevel.Debug;

            if (level == Level.Warn)
                return LogLevel.Warning;

            if (level == Level.Error)
                return LogLevel.Error;

            if (level == Level.Fatal)
                return LogLevel.Fatal;

            return LogLevel.Info;
        }

        #endregion
    }
}