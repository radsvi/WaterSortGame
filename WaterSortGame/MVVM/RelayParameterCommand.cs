using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WaterSortGame.MVVM
{
    internal class RelayParameterCommand : ICommand
    {
        private Action<object, string> execute;
        private Func<object, bool> canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayParameterCommand(Action<object, string> execute, Func<object, bool> canExecute = null, object? parameter = null)
        //public RelayParameterCommand(Action<object> execute, Func<object, bool> canExecute = null)
        //public RelayParameterCommand(Action<object, string> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            //var neco = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            execute(parameter, "neco");
        }
    }
}
