using System;
using DynamicData;

namespace Trader.Domain.Infrastucture
{

    public class LogEntry: IKey<long>, IEquatable<LogEntry>
    {
        public LogEntry(long counter, string message, string loggerName, string threadName, DateTime timeStamp, LogLevel level, Exception exception)
        {
            Counter = counter;
            Message = message;
            LoggerName = loggerName;
            ThreadName = threadName;
            TimeStamp = timeStamp;
            Level = level;
            Exception = exception;
        }

        public string Message { get; }

        public string LoggerName { get; }

        public string ThreadName { get; }

        public DateTime TimeStamp { get; }

        public LogLevel Level { get; }

        public Exception Exception { get; }

        public long Counter { get; }

        public long Key => Counter;

        #region Equality members

        public bool Equals(LogEntry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Counter == other.Counter;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LogEntry) obj);
        }

        public override int GetHashCode()
        {
            return Counter.GetHashCode();
        }

        public static bool operator ==(LogEntry left, LogEntry right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LogEntry left, LogEntry right)
        {
            return !Equals(left, right);
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return $"Level: {Level}, ThreadName: {ThreadName}, LoggerName: {LoggerName}, Message: {Message}, TimeStamp: {TimeStamp}";
        }
    }
}