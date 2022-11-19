using System.Windows;
using MaterialDesignExtensions.Controls;

namespace Trader.Client.Infrastructure;

public class DynamicTraderWindow : MaterialWindow
{
    static DynamicTraderWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicTraderWindow), new FrameworkPropertyMetadata(typeof(DynamicTraderWindow)));
    }

    public static readonly DependencyProperty LeftHeaderContentProperty = DependencyProperty.Register(
        "LeftHeaderContent", typeof(object), typeof(DynamicTraderWindow), new PropertyMetadata(default(object)));

    public static void SetLeftHeaderContent(DependencyObject element, object value)
    {
        element.SetValue(LeftHeaderContentProperty, value);
    }

    public static object GetLeftHeaderContent(DependencyObject element)
    {
        return (object)element.GetValue(LeftHeaderContentProperty);
    }

    public static readonly DependencyProperty RightHeaderContentProperty = DependencyProperty.Register(
        "RightHeaderContent", typeof(object), typeof(DynamicTraderWindow), new PropertyMetadata(default(object)));

    public static void SetRightHeaderContent(DependencyObject element, object value)
    {
        element.SetValue(RightHeaderContentProperty, value);
    }

    public static object GetRightHeaderContent(DependencyObject element)
    {
        return (object)element.GetValue(RightHeaderContentProperty);
    }
}