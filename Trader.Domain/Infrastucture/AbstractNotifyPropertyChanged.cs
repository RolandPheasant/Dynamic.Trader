using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using DynamicData;
using DynamicData.Kernel;
using TradeExample.Annotations;

namespace Trader.Domain.Infrastucture
{
    public static class ObservableCollectionEx
    {
        public static IObservable<IChangeSet<TObject, TKey>> WatchChanges<TObject, TKey>(this ObservableCollection<TObject> source, Func<TObject, TKey> keySelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");


            return Observable.Create<IChangeSet<TObject, TKey>>
                (
                    observer =>
                    {

                      //  var sourceCache = new SourceCache<TObject, TKey>(keySelector);

                        var observableCollection = Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(source, "CollectionChanged");

                        var observableCollectionUpdates = observableCollection.Select
                            (
                                args =>
                                {

                                    var changes = args.EventArgs;

                                    switch (changes.Action)
                                    {
                                        case NotifyCollectionChangedAction.Add:
                                            return changes.NewItems.OfType<TObject>()
                                                .Select(
                                                    t => new Change<TObject, TKey>(ChangeReason.Add, keySelector(t), t));

                                        case NotifyCollectionChangedAction.Remove:
                                            return changes.OldItems.OfType<TObject>()
                                                .Select(
                                                    t =>
                                                        new Change<TObject, TKey>(ChangeReason.Remove, keySelector(t), t));

                                        case NotifyCollectionChangedAction.Replace:
                                            {
                                                return changes.NewItems.OfType<TObject>()
                                                    .Select((t, idx) =>
                                                    {
                                                        var old = changes.OldItems[idx];
                                                        return new Change<TObject, TKey>(ChangeReason.Update, keySelector(t), t, (TObject)old);
                                                    });
                                            }

                                        case NotifyCollectionChangedAction.Reset:
                                            {
                                                //TODO: need to store current updates so we can clear the old items
                                                var removes = source.Select(t => new Change<TObject, TKey>(ChangeReason.Remove, keySelector(t), t));



                                            }
                                            return null;
                                        default:
                                            return null;
                                    }
                                })
                                .Where(updates => updates != null)
                                .Select(updates => (IChangeSet<TObject, TKey>)new ChangeSet<TObject, TKey>(updates));

                        var initial = source.Select(t => new Change<TObject, TKey>(ChangeReason.Add, keySelector(t), t));
                        var first = (IChangeSet<TObject, TKey>)new ChangeSet<TObject, TKey>(initial);


                        return Observable.Return<IChangeSet<TObject, TKey>>(first).Concat(observableCollectionUpdates).SubscribeSafe(observer);
                    }

                );
        }

    }

    public abstract class AbstractNotifyPropertyChanged: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
