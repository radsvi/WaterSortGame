using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WaterSortGame.ViewModels;

namespace WaterSortGame.MVVM
{ // https://www.youtube.com/watch?v=fOookEq5od0
    public class SimpleCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public ViewModelBaseSimple ViewModel { get; set; }
        public SimpleCommand(ViewModelBaseSimple viewModel)
        {
            this.ViewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            this.ViewModel.SimpleMethod();
        }
    }
}
