using System.Windows;
using System.Windows.Controls.Primitives;

namespace Trader.Client.Infrastucture
{
    public static class SelectorHelper
    {
        public static readonly DependencyProperty BindingProperty = DependencyProperty.RegisterAttached("Binding", typeof(SelectionController), typeof(SelectorHelper),
                new PropertyMetadata(default(SelectionController), PropertyChanged));

        public static void SetBinding(Selector element, SelectionController value)
        {
            element.SetValue(BindingProperty, value);
        }

        public static SelectionController GetBinding(Selector element)
        {
            return (SelectionController)element.GetValue(BindingProperty);
        }

        public static void PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var receiver = args.NewValue as SelectionController;
            if (receiver == null) return;
            receiver.Receive((Selector)sender);
        }
    }
}
