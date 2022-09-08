/* 
    Author: Derek Ackworth
    Purpose: Execute commands
*/

using System;
using System.Windows.Input;

namespace InvestmentPortfolio.Bases
{
    public class CommandBase : ICommand
    {
        private readonly Action<object> _execute;

        public CommandBase(Action<object> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
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

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
