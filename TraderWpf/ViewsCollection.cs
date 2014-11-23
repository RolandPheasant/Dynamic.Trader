using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Dragablz;
using DynamicData;
using DynamicData.Binding;

namespace TraderWpf
{
    public class ViewsCollection: IDisposable
    {
        private readonly IInterTabClient _interTabClient;
        //private readonly ObservableCollection<ViewContainer> _items;
        private readonly IDisposable _cleanup;

       private readonly ISourceCache<ViewContainer, Guid> _views = new SourceCache<ViewContainer, Guid>(vc=>vc.Id);
       private readonly IObservableCollection<ViewContainer> _data = new ObservableCollectionExtended<ViewContainer>();
      
        public ViewsCollection()
        {
            //_items = new ObservableCollection<ViewContainer>();
            _interTabClient =   new InterTabClient();

            var loader = _views.Connect()
                .ObserveOnDispatcher()
                .Bind(_data)
                .Subscribe();

            _data.CollectionChanged += _data_CollectionChanged;

            _cleanup = Disposable.Create(() =>
                                         {
                                             _views.Dispose();
                                             loader.Dispose();
                                         });
        }

        void _data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
       
        }

        public void Add(ViewContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            _views.AddOrUpdate(container);
        }

        public IObservableCollection<ViewContainer> Items
        {
            get { return _data; }
        }

        public static Guid TabPartition
        {
            get { return new Guid("2AE89D18-F236-4D20-9605-6C03319038E6"); }
        }

        public IInterTabClient InterTabClient
        {
            get { return _interTabClient; }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}