using System.Windows;
using System.Windows.Controls.Primitives;

namespace Trader.Client.Infrastucture
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
            var receiver = args.NewValue as IAttachedSelector;
            if (receiver == null) return;
            receiver.Receive((Selector)sender);
        }
    }
}
