using System.Collections.Generic;

namespace DynamicData.Common.Logging
{
    public interface ILogEntryService
    {
        IObservableCache<LogEntry, long> Cache { get; }

        void Remove(IEnumerable<LogEntry> items);
        void Remove(IEnumerable<long> keys);

    }
}
