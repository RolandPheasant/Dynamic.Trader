using System;

namespace Trader.Domain.Infrastucture
{
    public class LogEntrySummary : IEquatable<LogEntrySummary>
    {
        private readonly int _debug;
        private readonly int _info;
        private readonly int _warning;
        private readonly int _error;

        public readonly static LogEntrySummary Empty = new LogEntrySummary();

        private LogEntrySummary()
        {
        }

        public LogEntrySummary(int debug, int info, int warning, int error)
        {
            _debug = debug;
            _info = info;
            _warning = warning;
            _error = error;
        }

        public int Debug
        {
            get { return _debug; }
        }

        public int Info
        {
            get { return _info; }
        }

        public int Warning
        {
            get { return _warning; }
        }

        public int Error
        {
            get { return _error; }
        }

        #region Equality members

        public bool Equals(LogEntrySummary other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _error == other._error && _warning == other._warning && _info == other._info && _debug == other._debug;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LogEntrySummary)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _error;
                hashCode = (hashCode * 397) ^ _warning;
                hashCode = (hashCode * 397) ^ _info;
                hashCode = (hashCode * 397) ^ _debug;
                return hashCode;
            }
        }

        public static bool operator ==(LogEntrySummary left, LogEntrySummary right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LogEntrySummary left, LogEntrySummary right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}