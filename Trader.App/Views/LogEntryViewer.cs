using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using DynamicData.Operators;
using DynamicData.ReactiveUI;
using ReactiveUI;
using Trader.Client.Infrastucture;
using Trader.Domain.Infrastucture;

namespace Trader.Client.Views
{
    public class LogEntryProxy : ReactiveObject, IIndexAware, ISelectedAware
    {
        private readonly LogEntry _original;
        private bool _isSelected;
        private int _index;

        public LogEntryProxy(LogEntry original)
        {
            _original = original;
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref _isSelected, value);}
        }


        public int Index
        {
            get { return _index; }
            set { this.RaiseAndSetIfChanged(ref _index, value); }
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
    }

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

    public class LogEntryTile : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IDisposable _cleanUp;
        private LogEntrySummary _summary = LogEntrySummary.Empty;

        public LogEntryTile(ILogEntryService service, ILogger logger)
        {
            _logger = logger;

            var scanner = service.Items.Connect()
                                    .Batch(TimeSpan.FromMilliseconds(250))
                                    .QueryWhenChanged(query =>
                                    {
                                        var items = query.Items.ToList();
                                        var debug = items.Count(le => le.Level == LogLevel.Debug);
                                        var info = items.Count(le => le.Level == LogLevel.Info);
                                        var warn = items.Count(le => le.Level == LogLevel.Warning);
                                        var error = items.Count(le => le.Level == LogLevel.Error);
                                        return new LogEntrySummary(debug, info, warn, error);
                                    })
                                    .Subscribe(s => Summary = s);

            _cleanUp = scanner;
        }


        public LogEntrySummary Summary
        {
            get { return _summary; }
            set { SetAndRaise(ref _summary, value);}
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }


    }

    public class LogEntryViewer : ReactiveObject, IDisposable
    {
        private readonly ILogEntryService _logEntryService;
        private readonly FilterController<LogEntry> _filter = new FilterController<LogEntry>(l => true);

        private readonly ReactiveList<LogEntryProxy> _data = new ReactiveList<LogEntryProxy>();
        private readonly SelectorBinding _multiSelectorBinding = new SelectorBinding();
        private bool _paused;

        private readonly IDisposable _cleanUp;
        private string _searchText=string.Empty;


        public LogEntryViewer(ILogEntryService logEntryService)
        {
            _logEntryService = logEntryService;

            var filterApplier = this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(BuildFilter)
                .Subscribe(_filter.Change);

            var loader = logEntryService.Items.Connect(_filter)
               // .BatchIf(this.WhenAnyObservable(x => x.Paused))
                .Transform(le => new LogEntryProxy(le))
                .Sort(SortExpressionComparer<LogEntryProxy>.Descending(l => l.Key), SortOptimisations.ComparesImmutableValuesOnly)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(_data)
                .Subscribe();

            _cleanUp= Disposable.Create(() =>
            {
                loader.Dispose();
                filterApplier.Dispose();
                _filter.Dispose();
            });
        }

        private Func<LogEntry, bool> BuildFilter(string searchText)
        {
            if (string.IsNullOrEmpty(SearchText))
                return le => true;

            return le => le.Message.ToLowerInvariant().Contains(SearchText.ToLowerInvariant());
        }

        public bool Paused
        {
            get { return _paused; }
            set { this.RaiseAndSetIfChanged(ref _paused, value); } 
        }

        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); } 
        }

        public SelectorBinding MultiSelector
        {
            get { return _multiSelectorBinding; }
        }

        public ReactiveList<LogEntryProxy> Data
        {
            get { return _data; }
        }

        public void Dispose()
        {
           _cleanUp.Dispose();
        }
    }
}
