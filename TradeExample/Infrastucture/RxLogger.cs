using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using log4net.Appender;
using log4net.Core;

namespace DynamicData.Common.Logging
{
    public class RxAppender : AppenderSkeleton
    {
        private readonly static ISubject<LogEntry> _subject = new Subject<LogEntry>();
        private  long _counter;
        private int _queueSize;

        public static IObservable<LogEntry> LogEntryObservable
        {
            get { return _subject.AsObservable(); }
        }

        #region Overrides of AppenderSkeleton


        public int QueueSize
        {
            get { return _queueSize; }
            set { _queueSize = value; }
        }


        protected override void OnClose()
        {
            _subject.OnCompleted();
            base.OnClose();
        }


        protected override void Append(LoggingEvent loggingEvent)
        {
            //TODO: is this thread safe?
            Interlocked.Increment(ref _counter);
           
            var entry = new LogEntry(_counter,
                                        loggingEvent.RenderedMessage,
                                        loggingEvent.LoggerName,
                                        loggingEvent.ThreadName,
                                        loggingEvent.TimeStamp,
                                        Map(loggingEvent.Level),
                                        loggingEvent.ExceptionObject);

            _subject.OnNext(entry);
        }

        private  LogLevel Map(Level level)
        {
            if (level==Level.Info)
                return  LogLevel.Info;

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
