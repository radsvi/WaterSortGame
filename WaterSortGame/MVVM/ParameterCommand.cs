using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WaterSortGame.ViewModels;

namespace WaterSortGame.MVVM
{
    internal class ParameterCommand : ICommand
    {
        public ViewModelBaseSimple ViewModel { get; set; }
        public ParameterCommand(ViewModelBaseSimple viewModel)
        {
            this.ViewModel = viewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (parameter != null)
            {
                var s = parameter as String;
                if (string.IsNullOrEmpty(s))
                    return false;

                return true;
            }
            return false;
        }

        public void Execute(object? parameter)
        {
            this.ViewModel.ParameterMethod(parameter as String);
        }
    }
}
