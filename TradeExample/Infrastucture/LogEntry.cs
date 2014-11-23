using System;

namespace DynamicData.Common.Logging
{
    public class LogEntry: IKey<long>
    {
        private readonly long _counter;
        private readonly string _message;
        private readonly string _loggerName;
        private readonly string _threadName;
        private readonly DateTime _timeStamp;
        private readonly LogLevel _level;
        private readonly Exception _exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public LogEntry(long counter, string message, string loggerName, string threadName, DateTime timeStamp, LogLevel level, Exception exception)
        {
            _counter = counter;
            _message = message;
            _loggerName = loggerName;
            _threadName = threadName;
            _timeStamp = timeStamp;
            _level = level;
            _exception = exception;
        }

        public string Message
        {
            get { return _message; }
        }

        public string LoggerName
        {
            get { return _loggerName; }
        }

        public string ThreadName
        {
            get { return _threadName; }
        }

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
        }

        public LogLevel Level
        {
            get { return _level; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        public long Counter
        {
            get { return _counter; }
        }

        #region Implementation of IKey<out long>

        public long Key
        {
            get { return _counter; }
        }

        #endregion

        #region Equality members

        protected bool Equals(LogEntry other)
        {
            return _counter == other._counter;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LogEntry) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return _counter.GetHashCode();
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
            return string.Format("Level: {0}, ThreadName: {1}, LoggerName: {2}, Message: {3}, TimeStamp: {4}", _level, _threadName, _loggerName, _message, _timeStamp);
        }
    }
}