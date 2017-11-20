using System;
using DynamicData.Binding;

namespace Trader.Client.Infrastucture
{
    public class ViewContainer: AbstractNotifyPropertyChanged
    {
        public ViewContainer(string title, object content)
        {
            Title = title;
            Content = content;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public string Title { get; }
        public object Content { get; }
    }
}