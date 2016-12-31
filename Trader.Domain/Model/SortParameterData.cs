using System.Collections.Generic;
using DynamicData.Binding;

namespace Trader.Domain.Model
{
    public class SortParameterData : AbstractNotifyPropertyChanged
    {
        private SortContainer _selectedItem;
        private readonly IList<SortContainer> _sortItems = new List<SortContainer>
        {
            new SortContainer("Customer, Currency Pair", SortExpressionComparer<TradeProxy>
                .Ascending(l => l.Customer)
                .ThenByAscending(p => p.CurrencyPair)
                .ThenByAscending(p => p.Id)),

            new SortContainer("Currency Pair, Amount", SortExpressionComparer<TradeProxy>
                .Ascending(l => l.CurrencyPair)
                .ThenByDescending(p => p.Amount)
                .ThenByAscending(p => p.Id)),

                           
            new SortContainer("Recently Changed", SortExpressionComparer<TradeProxy>
                .Descending(l => l.Timestamp)
                .ThenByAscending(p => p.Customer)
                .ThenByAscending(p => p.Id)),
        };


        public SortParameterData()
        {
            SelectedItem = _sortItems[2];
        }

        public SortContainer SelectedItem
        {
            get { return _selectedItem; }
            private set { SetAndRaise(ref _selectedItem, value); }
        }

        public IEnumerable<SortContainer> SortItems => _sortItems;
    }
}