using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

// ReSharper disable once CheckNamespace 
namespace DynamicData
{
    public static class DynamicDataEx
    {

        public static IObservable<IChangeSet<TObject, TKey>> DelayRemove<TObject, TKey>(this IObservable<IChangeSet<TObject, TKey>> source,
            TimeSpan delayPeriod, Action<TObject> onDefer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (onDefer == null) throw new ArgumentNullException("onDefer");

            return Observable.Create<IChangeSet<TObject, TKey>>(observer =>
            {

                var locker = new object();
                var shared = source.Publish();
                var notRemoved = shared.WhereReasonsAreNot(ChangeReason.Remove)
                    .Synchronize(locker);

                var removes = shared.WhereReasonsAre(ChangeReason.Remove)
                    .Do(changes => changes.Select(change => change.Current).ForEach(onDefer))
                    .Delay(delayPeriod)
                    .Synchronize(locker);

                var subscriber = notRemoved.Merge(removes).SubscribeSafe(observer);
                return new CompositeDisposable(subscriber, shared.Connect());

            });
        }
    }
}