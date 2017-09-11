using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Trader.Domain.Infrastucture;

namespace Trader.Client.Infrastucture
{
    public class LogEntryProxy : ReactiveObject, IDisposable, IEquatable<LogEntryProxy>
    {
        private readonly IDisposable _cleanUp;

        private readonly ObservableAsPropertyHelper<bool> _recent;
        private bool _removing;

        public LogEntryProxy(LogEntry original)
        {
            Original = original;

            //create a lazy observable property 
            _recent = Observable.Create<bool>(observer =>
            {
                var isRecent = DateTime.Now.Subtract(original.TimeStamp).TotalSeconds < 2;
                if (!isRecent) return Disposable.Empty;
                observer.OnNext(true);
                return Observable.Timer(TimeSpan.FromSeconds(2)).Select(_ => false).SubscribeSafe(observer);
            }).ToProperty(this, lep => lep.Recent);

            //dispose after use
            _cleanUp = _recent;
        }

        public bool Recent => _recent.Value;

        public bool Removing
        {
            get => _removing;
            set => this.RaiseAndSetIfChanged(ref _removing, value);
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }

        public void FlagForRemove()
        {
            Removing = true;
        }

        #region Delegated Members

        public string Message => Original.Message;

        public string LoggerName => Original.LoggerName;

        public string ThreadName => Original.ThreadName;

        public DateTime TimeStamp => Original.TimeStamp;

        public LogLevel Level => Original.Level;

        public long Key => Original.Key;

        public long Counter => Original.Counter;

        public Exception Exception => Original.Exception;

        public LogEntry Original { get; }

        #endregion

        #region Equality

        public bool Equals(LogEntryProxy other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Original.Equals(other.Original);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LogEntryProxy) obj);
        }

        public override int GetHashCode()
        {
            return Original.GetHashCode();
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
    }
}