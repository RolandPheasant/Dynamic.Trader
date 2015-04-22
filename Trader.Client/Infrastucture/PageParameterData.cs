using System.Windows.Input;
using DynamicData.Binding;
using DynamicData.Operators;

namespace Trader.Client.Infrastucture
{
    public class PageParameterData : AbstractNotifyPropertyChanged
    {
        private int _currentPage;
        private int _pageCount;
        private int _pageSize;
        private int _totalCount;
        private readonly Command _nextPageCommand;
        private readonly Command _previousPageCommand;

        public PageParameterData(int currentPage, int pageSize)
        {
            _currentPage = currentPage;
            _pageSize = pageSize;

            _nextPageCommand = new Command(() => CurrentPage = CurrentPage + 1, () => CurrentPage < PageCount);
            _previousPageCommand = new Command(() => CurrentPage = CurrentPage - 1, () => CurrentPage > 1);
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

        public ICommand NextPageCommand
        {
            get { return _nextPageCommand; }
        }

        public ICommand PreviousPageCommand
        {
            get { return _previousPageCommand; }
        }

        public int TotalCount
        {
            get { return _totalCount; }
            private set { SetAndRaise(ref _totalCount, value); }
        }

        public int PageCount
        {
            get { return _pageCount; }
            private set { SetAndRaise(ref _pageCount, value); }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
            private set { SetAndRaise(ref _currentPage, value); }
        }


        public int PageSize
        {
            get { return _pageSize; }
            private set { SetAndRaise(ref _pageSize, value); }
        }


    }
}