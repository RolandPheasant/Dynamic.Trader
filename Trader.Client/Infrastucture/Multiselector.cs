using System.Windows;
using System.Windows.Controls.Primitives;

namespace Trader.Client.Infrastucture
{
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
