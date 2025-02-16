using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WaterSortGame.Models;
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

            switch ((PopupParameters)parameter)
            {
                case PopupParameters.NewLevel:
                    viewModel.SelectedViewModel = new NewLevelVM(viewModel);
                    break;
                case PopupParameters.RestartLevel:
                    viewModel.SelectedViewModel = new RestartLevelVM(viewModel);
                    break;
                case PopupParameters.LevelComplete:
                    viewModel.SelectedViewModel = new LevelCompleteVM(viewModel);
                    break;
                case PopupParameters.Help:
                    viewModel.SelectedViewModel = new HelpVM(viewModel);
                    break;
                case PopupParameters.LoadLevel:
                    viewModel.SelectedViewModel = new LoadLevelVM(viewModel);
                    break;
                case PopupParameters.GameSaved:
                    viewModel.SelectedViewModel = new GameSavedVM(viewModel);
                    break;
                case PopupParameters.CloseNotification: // closing GameSavedScreen
                    viewModel.SelectedViewModel = null;
                    //
                    break;
                default:
                    viewModel.SelectedViewModel = null;
                    break;
            }
        }
    }
}