namespace Trader.Client.Infrastucture
{
    public class Link
    {
        private readonly string _text;
        private readonly string _display;
        private readonly string _url;

        public Link(string text, string url)
            : this(text,url,url)
        {
            _text = text;
            _url = url;
        }

        public Link(string text,string display, string url)
        {
            _text = text;
            _display = display;
            _url = url;
        }


        public string Text
        {
            get { return _text; }
        }

        public string Url
        {
            get { return _url; }
        }

        public string Display
        {
            get { return _display; }
        }
    }
}