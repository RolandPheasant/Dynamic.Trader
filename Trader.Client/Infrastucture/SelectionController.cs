using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DynamicData;

namespace Trader.Client.Infrastucture
{
    public class SelectionController<T> : IDisposable, IAttachedSelector
    {
        private readonly ISourceList<T> _sourceList = new SourceList<T>();
        private readonly IDisposable _cleanUp;
        private readonly SerialDisposable _serialDisposable = new SerialDisposable();

        private bool _isSelecting;
        private Selector _selector;
    

        public SelectionController()
        {
            _cleanUp = new CompositeDisposable(_sourceList, _serialDisposable);
        }

        void IAttachedSelector.Receive(Selector selector)
        {
            _selector = selector;

            _serialDisposable.Disposable = Observable
                .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(
                    h => selector.SelectionChanged += h,
                    h => selector.SelectionChanged -= h)
                .Select(evt => evt.EventArgs)
                .Subscribe(HandleSelectionChanged);
        }

        public IObservableList<T> SelectedItems => _sourceList;

        private void HandleSelectionChanged(SelectionChangedEventArgs args)
        {
            if (_isSelecting) return;
            try
            {
                _isSelecting = true;

                _sourceList.Edit(list =>
                {
                        list.AddRange(args.AddedItems.OfType<T>().ToList());
                        list.RemoveMany(args.RemovedItems.OfType<T>().ToList());
                });
            }
            finally
            {
                _isSelecting = false;
            }
        }

        public void Select(T item)
        {
            if (_selector == null) return;

            if (!_selector.Dispatcher.CheckAccess())
            {
                _selector.Dispatcher.BeginInvoke(new Action(() => Select(item)));
                return;
            }


            if (_selector is ListView)
            {
                ((ListView)_selector).SelectedItems.Add(item);
            }
            else if (item is MultiSelector)
            {
                ((MultiSelector)_selector).SelectedItems.Add(item);
            }
            else
            {
                _selector.SelectedItem = item;
            }

        }

        public void DeSelect(T item)
        {
            if (_selector == null) return;
            if (!_selector.Dispatcher.CheckAccess())
            {
                _selector.Dispatcher.BeginInvoke(new Action(() => DeSelect(item)));
                return;
            }

            if (_selector is ListView)
            {
                ((ListView)_selector).SelectedItems.Remove(item);
            }
            else if (_selector is MultiSelector)
            {
                ((MultiSelector)_selector).SelectedItems.Remove(item);
            }
            else
            {
                _selector.SelectedItem = null;
            }
        }

        public void Clear()
        {
            if (_selector == null) return;
            if (!_selector.Dispatcher.CheckAccess())
            {
                _selector.Dispatcher.BeginInvoke(new Action(Clear));
                return;
            }

            if (_selector is ListView)
            {
                ((ListView)_selector).SelectedItems.Clear();
            }
            else if (_selector is MultiSelector)
            {
                ((MultiSelector)_selector).SelectedItems.Clear();
            }
            else
            {
                _selector.SelectedItem = null;
            }
        }





        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}