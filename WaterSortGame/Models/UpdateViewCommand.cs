using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WaterSortGame.ViewModels;
using WaterSortGame.Views;

namespace WaterSortGame.MVVM
{
    internal class UpdateViewCommand : ICommand
    {
        private MainWindowVM viewModel;
        public UpdateViewCommand(MainWindowVM viewModel)
        {
            this.viewModel = viewModel;
        }
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter.ToString() == "NewLevel")
            {
                viewModel.SelectedViewModel = new NewLevelVM();
            }
            else if (parameter.ToString() == "RestartLevel")
            {
                viewModel.SelectedViewModel = new RestartLevelVM();
            }
        }
    }
}