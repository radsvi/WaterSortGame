using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WaterSortGame.ViewModels;

namespace WaterSortGame.MVVM
{
    internal class PopupWindowCommand : ICommand
    {
        private MainWindowVM viewModel;
        public PopupWindowCommand(MainWindowVM viewModel)
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
            if (parameter is null)
            {
                viewModel.SelectedViewModel = null;
                return;
            }
            if (parameter.ToString() == "NewLevel")
            {
                viewModel.SelectedViewModel = new NewLevelVM(viewModel);
            }
            else if (parameter.ToString() == "RestartLevel")
            {
                viewModel.SelectedViewModel = new RestartLevelVM(viewModel);
            }
            else if (parameter.ToString() == "LevelComplete")
            {
                viewModel.SelectedViewModel = new LevelCompleteVM(viewModel);
            }
            else if (parameter.ToString() == "Help")
            {
                viewModel.SelectedViewModel = new HelpVM(viewModel);
            }
        }
    }
}