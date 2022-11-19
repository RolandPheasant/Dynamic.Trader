using System.Windows;
using System.Windows.Controls.Primitives;

namespace Trader.Client.Infrastructure
{
    public static class SelectorHelper
    {
        public static readonly DependencyProperty BindingProperty = DependencyProperty.RegisterAttached("Binding", typeof(IAttachedSelector), typeof(SelectorHelper),
                new PropertyMetadata(default(IAttachedSelector), PropertyChanged));

        public static void SetBinding(Selector element, IAttachedSelector value)
        {
            element.SetValue(BindingProperty, value);
        }

        public static IAttachedSelector GetBinding(Selector element)
        {
            return (IAttachedSelector)element.GetValue(BindingProperty);
        }

        public static void PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (!(args.NewValue is IAttachedSelector receiver)) return;
            receiver.Receive((Selector)sender);
        }
    }
}
