﻿using System;
using System.Windows.Input;

namespace Trader.Client.Infrastructure
{
    /// <summary>
    /// A command wich accepts no parameter - assumes the view model will do the work
    /// </summary>
    public class Command : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;


        public Command(Action execute, Func<bool> canExecute = null)
        {
            if (execute == null) throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute ?? (() => true);
        }


        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void Refresh()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
