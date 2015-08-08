using System.Collections.Generic;
using DynamicData;

namespace Trader.Domain.Infrastucture
{
    public interface ILogEntryService
    {
        IObservableList<LogEntry> Items { get; }
        void Remove(IEnumerable<LogEntry> items);
    }
}
