using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Controllers;
using DynamicData.ReactiveUI;
using ReactiveUI;
using Trader.Client.Infrastucture;
using Trader.Domain.Infrastucture;

namespace Trader.Client.Views
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

          //  Removing = true;
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
            return Equals((LogEntryProxy) obj);
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

    public static class DynamicDataEx
    {

        public static IObservable<IChangeSet<TObject, TKey>> DelayRemove<TObject, TKey>(this IObservable<IChangeSet<TObject, TKey>> source,
            Action<TObject> onDefer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (onDefer == null) throw new ArgumentNullException("onDefer");

            return Observable.Create<IChangeSet<TObject, TKey>>(observer =>
            {

                var locker = new object();
                var shared = source.Publish();
                var notRemoved = shared.WhereReasonsAreNot(ChangeReason.Remove)
                        .Synchronize(locker);

                var removes = shared.WhereReasonsAre(ChangeReason.Remove)
                    .Do(changes => changes.Select(change => change.Current).ForEach(onDefer))
                    .Delay(TimeSpan.FromSeconds(0.75))
                    .Synchronize(locker);

                var subscriber = notRemoved.Merge(removes).SubscribeSafe(observer);
                var connected = shared.Connect();
                return new CompositeDisposable(subscriber, connected);

            });
        }
    }

    public class LogEntryViewer : ReactiveObject, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly ILogEntryService _logEntryService;
        private readonly FilterController<LogEntryProxy> _filter = new FilterController<LogEntryProxy>(l => true);
        private readonly ReactiveList<LogEntryProxy> _data = new ReactiveList<LogEntryProxy>();
        private readonly SelectorBinding _selection = new SelectorBinding();
        private readonly ReactiveCommand<object> _deleteCommand;
        private LogEntrySummary _summary = LogEntrySummary.Empty;
        private string _searchText=string.Empty;
        private string _removeText;
       
        public LogEntryViewer(ILogEntryService logEntryService)
        {
            _logEntryService = logEntryService;


            //apply filter when search text has changed
            var filterApplier = this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(BuildFilter)
                .Subscribe(_filter.Change);

            //filter, sort and populate reactive list.
            var loader = logEntryService.Items.Connect()
                .Transform(le => new LogEntryProxy(le))
                .DelayRemove(proxy =>
                {
                    proxy.FlagForRemove();
                    _selection.DeSelect(proxy);
                })
                .Filter(_filter)
                .Sort(SortExpressionComparer<LogEntryProxy>.Descending(l => l.Key))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(_data)
                .DisposeMany()
                .Subscribe();

            //aggregate total items
            var summariser = logEntryService.Items.Connect()
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

            //manage user selection, delete items command
            var selectedItems = _selection.Selected.ToObservableChangeSet().Transform(obj => (LogEntryProxy)obj).Publish();
            
            //Build a message from selected items
            var selectedMessage = selectedItems
                                        .QueryWhenChanged(query =>
                                        {
                                            if (query.Count == 0) return "Select log entries to delete";
                                            if (query.Count == 1) return "Delete selected log entry";
                                            return string.Format("Delete {0} log entries", query.Count);
                                        })
                                        .StartWith("Select log entries to delete")
                                        .Subscribe(text=>RemoveText=text);

            //covert stream into a cache so we can get a handle on items in thread safe manner. we could use the _data
            //but that is only thread safe if all the code is called from MainThread
            var selectedCache = selectedItems.AsObservableCache();

            //make a command out of selected items - enabling command when there is a selection 
            _deleteCommand = selectedItems
                                    .QueryWhenChanged(query => query.Count > 0)
                                    .ToCommand();

            //Assign action when the command is invoked
           var commandInvoker =  this.WhenAnyObservable(x => x.DeleteCommand)
                    .Subscribe(_ =>
                    {
                        var toRemove = selectedCache.Items.Select(proxy => proxy.Key).ToArray();
                        _logEntryService.Remove(toRemove);
                    });

            var connected = selectedItems.Connect();

            _cleanUp= Disposable.Create(() =>
            {
                loader.Dispose();
                filterApplier.Dispose();
                _filter.Dispose();
                connected.Dispose();
                selectedMessage.Dispose();
                selectedCache.Dispose();
                _selection.Dispose();
                commandInvoker.Dispose();
                summariser.Dispose();
            });
        }

        private Func<LogEntryProxy, bool> BuildFilter(string searchText)
        {
            if (string.IsNullOrEmpty(SearchText))
                return le => true;

            return le => le.Message.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                            || le.Level.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        }

        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); } 
        }

        public string RemoveText
        {
            get { return _removeText; }
            set { this.RaiseAndSetIfChanged(ref _removeText , value); }
        }

        public LogEntrySummary Summary
        {
            get { return _summary; }
            set { this.RaiseAndSetIfChanged(ref _summary, value); }
        }

        public ReactiveList<LogEntryProxy> Data
        {
            get { return _data; }
        }
        
        public ReactiveCommand<object> DeleteCommand
        {
            get { return _deleteCommand; }
        }

        public SelectorBinding Selector
        {
            get { return _selection; }
        }


        public void Dispose()
        {
           _cleanUp.Dispose();
        }
    }
}
