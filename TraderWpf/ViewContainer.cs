using System;

namespace TraderWpf
{
    public class ViewContainer
    {
        private readonly Guid _id = Guid.NewGuid();
        private readonly string _title;
        private readonly object _content;

        public ViewContainer(string title, object content)
        {
            _title = title;
            _content = content;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public string Title
        {
            get { return _title; }
        }


        public object Content
        {
            get { return _content; }
        }
    }
}