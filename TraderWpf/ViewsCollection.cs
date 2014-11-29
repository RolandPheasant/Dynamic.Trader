using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Dragablz;
using DynamicData;
using DynamicData.Binding;
using TradeExample.Infrastucture;

namespace TraderWpf
{
    public class ViewsCollection:AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanup;

       private readonly ISourceCache<ViewContainer, Guid> _views = new SourceCache<ViewContainer, Guid>(vc=>vc.Id);
     //  private readonly IObservableCollection<ViewContainer> _data = new ObservableCollectionExtended<ViewContainer>();
        private ViewContainer _selected;
        private readonly ObservableCollection<ViewContainer> _data = new ObservableCollection<ViewContainer>();
        public ViewsCollection()
        {
            //var loader = _views.Connect()
            //    .ObserveOnDispatcher()
            //    .Bind(_data)
            //    .Subscribe();

          // _cleanup = new CompositeDisposable(_views, loader); 
        }
        
        public void Add(ViewContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            _data.Add(container);
            _views.AddOrUpdate(container);
          //  Selected = container;
        }

        //public ViewContainer Selected
        //{
        //    get { return _selected; }
        //    set
        //    {
        //        _selected = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public IObservableCollection<ViewContainer> Items
        //{
        //    get { return _data; }
        //}


        public ObservableCollection<ViewContainer> Items
        {
            get { return _data; }
        }

        public void Dispose()
        {
           _cleanup.Dispose();
        }
    }
}