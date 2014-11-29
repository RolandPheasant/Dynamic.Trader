using System;
using System.Windows.Input;

namespace TraderWpf
{
    public class Command : ICommand
    {
        private readonly Action _action;


        public Command(Action action)
        {
            _action = action;
        }

        #region Implementation of ICommand

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
