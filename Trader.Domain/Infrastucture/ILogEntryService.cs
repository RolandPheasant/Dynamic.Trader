using System.Collections.Generic;
using DynamicData;

namespace Trader.Domain.Infrastucture
{
    public interface ILogEntryService
    {
        IObservableCache<LogEntry, long> Items { get; }


        void Add(LogEntry items);
        void Remove(IEnumerable<LogEntry> items);
        void Remove(IEnumerable<long> keys);

    }
}
