using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Trader.Domain.Infrastucture;

namespace Trader.Client.Infrastucture
{
    public class LogEntryProxy : ReactiveObject, IDisposable, IEquatable<LogEntryProxy>
    {
        private readonly LogEntry _original;
        private readonly IDisposable _cleanUp = Disposable.Empty;
        private bool _recent;
        private bool _removing;

        public LogEntryProxy(LogEntry original)
        {
            _original = original;

            var isRecent = DateTime.Now.Subtract(original.TimeStamp).TotalSeconds < 2;
            if (!isRecent) return;

            Recent = true;
            _cleanUp = Observable.Timer(TimeSpan.FromSeconds(2))
                .Subscribe(_ => Recent = false);
        }

        public bool Recent
        {
            get { return _recent; }
            set { this.RaiseAndSetIfChanged(ref _recent, value); }
        }

        public bool Removing
        {
            get { return _removing; }
            set { this.RaiseAndSetIfChanged(ref _removing, value); }
        }

        public void FlagForRemove()
        {
            Removing = true;
        }

        #region Delegated Members

        public string Message
        {
            get { return _original.Message; }
        }

        public string LoggerName
        {
            get { return _original.LoggerName; }
        }

        public string ThreadName
        {
            get { return _original.ThreadName; }
        }

        public DateTime TimeStamp
        {
            get { return _original.TimeStamp; }
        }

        public LogLevel Level
        {
            get { return _original.Level; }
        }

        public long Key
        {
            get { return _original.Key; }
        }

        public long Counter
        {
            get { return _original.Counter; }
        }

        public Exception Exception
        {
            get { return _original.Exception; }
        }

        #endregion

        #region Equality

        public bool Equals(LogEntryProxy other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _original.Equals(other._original);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LogEntryProxy)obj);
        }

        public override int GetHashCode()
        {
            return _original.GetHashCode();
        }

        public static bool operator ==(LogEntryProxy left, LogEntryProxy right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LogEntryProxy left, LogEntryProxy right)
        {
            return !Equals(left, right);
        }

        #endregion

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}