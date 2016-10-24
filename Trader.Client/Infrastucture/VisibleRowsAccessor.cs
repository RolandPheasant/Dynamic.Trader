using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using DynamicData;

namespace Trader.Client.Infrastucture
{
    public interface IVisibleRowsAccessor<T>: IDisposable
    {
        IObservableList<T> VisibleRows { get; }
    }

    public class VisibleRowsAccessor<T> : IDependencyObjectReceiver, IVisibleRowsAccessor<T>
    {
        private DataGrid _grid;
        private readonly ISourceList<T> _visibleRowsSource = new SourceList<T>();
        private readonly IObservableList<T> _visibleRows; 
        private readonly SingleAssignmentDisposable _cleanUp = new SingleAssignmentDisposable();

        public VisibleRowsAccessor()
        {
            _visibleRows = _visibleRowsSource.AsObservableList();
        }

        public IObservableList<T> VisibleRows => _visibleRows;

        void IDependencyObjectReceiver.Receive(DependencyObject value)
        {
            var grid = value as DataGrid;
            if (grid == null) return;
            _grid = grid;

            var rowsAdded = Observable.FromEventPattern<DataGridRowEventArgs>
                (
                    ev => _grid.LoadingRow += ev,
                    ev => _grid.LoadingRow -= ev
                ).Select(e => e.EventArgs.Row)
                .Subscribe(datagridrow => _visibleRowsSource.Add((T)datagridrow.Item)); ;


            var rowsUnloaded = Observable.FromEventPattern<DataGridRowEventArgs>
                (
                    ev => _grid.UnloadingRow += ev,
                    ev => _grid.UnloadingRow -= ev
                ).Select(e => e.EventArgs.Row)
                .Subscribe(datagridrow => _visibleRowsSource.Remove((T)datagridrow.Item));

            _cleanUp.Disposable = new CompositeDisposable(rowsAdded, rowsUnloaded, _visibleRows, _visibleRowsSource);
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}