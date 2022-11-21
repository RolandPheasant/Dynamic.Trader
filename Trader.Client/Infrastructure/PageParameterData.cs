using System.Windows.Input;
using DynamicData.Binding;
using DynamicData.Operators;

namespace Trader.Client.Infrastructure;

public class PageParameterData : AbstractNotifyPropertyChanged
{
    private readonly Command _nextPageCommand;
    private readonly Command _previousPageCommand;
    private int _currentPage;
    private int _pageCount;
    private int _pageSize;
    private int _totalCount;

    public PageParameterData(int currentPage, int pageSize)
    {
        _currentPage = currentPage;
        _pageSize = pageSize;

        _nextPageCommand = new Command(() => CurrentPage = CurrentPage + 1, () => CurrentPage < PageCount);
        _previousPageCommand = new Command(() => CurrentPage = CurrentPage - 1, () => CurrentPage > 1);
    }

    public ICommand NextPageCommand => _nextPageCommand;

    public ICommand PreviousPageCommand => _previousPageCommand;

    public int TotalCount
    {
        get => _totalCount;
        private set => SetAndRaise(ref _totalCount, value);
    }

    public int PageCount
    {
        get => _pageCount;
        private set => SetAndRaise(ref _pageCount, value);
    }

    public int CurrentPage
    {
        get => _currentPage;
        private set => SetAndRaise(ref _currentPage, value);
    }


    public int PageSize
    {
        get => _pageSize;
        private set => SetAndRaise(ref _pageSize, value);
    }


    public void Update(IPageResponse response)
    {
        CurrentPage = response.Page;
        PageSize = response.PageSize;
        PageCount = response.Pages;
        TotalCount = response.TotalSize;
        _nextPageCommand.Refresh();
        _previousPageCommand.Refresh();
    }
}