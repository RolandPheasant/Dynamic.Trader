using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Trader.Client.Infrastucture
{
    public class SelectorBinding : IDisposable
    {
        private readonly ObservableCollection<object> _selected = new ObservableCollection<object>();
        private readonly SerialDisposable _serialDisposable = new SerialDisposable();
        private bool _isSelecting;

        public void Receive(Selector selector)
        {
            _serialDisposable.Disposable = Observable
                .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(
                    h => selector.SelectionChanged += h,
                    h => selector.SelectionChanged -= h)
                .Select(evt => evt.EventArgs)
                .Subscribe(HandleSelectionChanged);
        }

        private void HandleSelectionChanged(SelectionChangedEventArgs args)
        {
            if (_isSelecting) return;
            try
            {
                _isSelecting = true;

                foreach (var item in args.AddedItems)
                    _selected.Add(item);

                foreach (var item in args.RemovedItems)
                    _selected.Remove(item);
            }
            finally
            {

                _isSelecting = false;
            }

        }
        public void Dispose()
        {
            _serialDisposable.Dispose();
        }
    }

    public static class SelectorHelper
    {
        public static readonly DependencyProperty BindingProperty = DependencyProperty.RegisterAttached("Binding", typeof(SelectorBinding), typeof(SelectorHelper),
                new PropertyMetadata(default(SelectorBinding), PropertyChanged));

        public static void SetBinding(Selector element, SelectorBinding value)
        {
            element.SetValue(BindingProperty, value);
        }

        public static SelectorBinding GetBinding(Selector element)
        {
            return (SelectorBinding)element.GetValue(BindingProperty);
        }

        public static void PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var receiver = args.NewValue as SelectorBinding;
            if (receiver == null) return;
            receiver.Receive((Selector)sender);
        }
    }
}
