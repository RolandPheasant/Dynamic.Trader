using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData.Binding;

namespace Trader.Domain.Model;

public class SortParameterData : AbstractNotifyPropertyChanged
{
    private readonly IList<SortContainer> _sortItems = new ObservableCollection<SortContainer>
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
            .ThenByAscending(p => p.Id))
    };

    private SortContainer _selectedItem;


    public SortParameterData()
    {
        SelectedItem = _sortItems[2];
    }

    public SortContainer SelectedItem
    {
        get => _selectedItem;
        set => SetAndRaise(ref _selectedItem, value);
    }

    public IEnumerable<SortContainer> SortItems => _sortItems;
}