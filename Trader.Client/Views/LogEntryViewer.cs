using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.ReactiveUI;
using ReactiveUI;
using Trader.Client.Infrastucture;
using Trader.Domain.Infrastucture;

namespace Trader.Client.Views
{
    public class LogEntryViewer : ReactiveObject, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly SelectionController<LogEntryProxy> _selectionController = new SelectionController<LogEntryProxy>();
        private readonly ObservableAsPropertyHelper<string> _deleteItemsText;

        private LogEntrySummary _summary = LogEntrySummary.Empty;
        private string _searchText = string.Empty;

        public LogEntryViewer(ILogEntryService logEntryService)
        {
            //build an observable filter
            var filter = this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(BuildFilter);
            
            //filter, sort and populate reactive list.
            var loader = logEntryService.Items.Connect()
                .Transform(le => new LogEntryProxy(le))
                .DelayRemove(TimeSpan.FromSeconds(0.75), proxy => proxy.FlagForRemove())
                .Filter(filter)
                .Sort(SortExpressionComparer<LogEntryProxy>.Descending(le => le.TimeStamp).ThenByDescending(l => l.Key), SortOptions.UseBinarySearch)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(Data)
                .DisposeMany()
                .Subscribe();

            //aggregate total items
            var summariser = logEntryService.Items.Connect()
                        .QueryWhenChanged(items =>
                        {
                            var debug = items.Count(le => le.Level == LogLevel.Debug);
                            var info = items.Count(le => le.Level == LogLevel.Info);
                            var warn = items.Count(le => le.Level == LogLevel.Warning);
                            var error = items.Count(le => le.Level == LogLevel.Error);
                            return new LogEntrySummary(debug, info, warn, error);
                        })
                        .Subscribe(s => Summary = s);

            //manage user selection, delete items command
            var selectedItems = _selectionController.SelectedItems.Connect().Publish();
            
            //Build a message from selected items
            _deleteItemsText = selectedItems.QueryWhenChanged(query =>
                {
                    if (query.Count == 0) return "Select log entries to delete";
                    if (query.Count == 1) return "Delete selected log entry?";
                    return $"Delete {query.Count} log entries?";
                })
                .ToProperty(this, viewmodel => viewmodel.DeleteItemsText, "Select log entries to delete");


            //make a command out of selected items - enabling the command when there is a selection 
            DeleteCommand = ReactiveCommand.Create(() =>
            {
                var toRemove = _selectionController.SelectedItems.Items.Select(proxy => proxy.Original).ToArray();
                _selectionController.Clear();
                logEntryService.Remove(toRemove);
            }, selectedItems.QueryWhenChanged(query => query.Count > 0));

           // .ToCommand();

            var connected = selectedItems.Connect();

            _cleanUp= Disposable.Create(() =>
            {
                loader.Dispose();
                connected.Dispose();
                _deleteItemsText.Dispose();
                DeleteCommand.Dispose();
                _selectionController.Dispose();
                summariser.Dispose();
            });
        }

        private Func<LogEntryProxy, bool> BuildFilter(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return logentry => true;

            return logentry => logentry.Message.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                            || logentry.Level.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase);
        }

        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); } 
        }

        public string DeleteItemsText => _deleteItemsText.Value;

        public LogEntrySummary Summary
        {
            get { return _summary; }
            set { this.RaiseAndSetIfChanged(ref _summary, value); }
        }

        public ReactiveList<LogEntryProxy> Data { get; } = new ReactiveList<LogEntryProxy>();

        public ReactiveCommand DeleteCommand { get; }

        public IAttachedSelector Selector => _selectionController;


        public void Dispose()
        {
           _cleanUp.Dispose();
        }
    }
}
