using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            Dictionary<PopupParams, ViewModelBase> viewModelsList = new Dictionary<PopupParams, ViewModelBase>
            {
                { PopupParams.NewLevel, new NewLevelVM(viewModel)},
                { PopupParams.RestartLevel, new RestartLevelVM(viewModel)},
                { PopupParams.LevelComplete, new LevelCompleteVM(viewModel)},
                { PopupParams.Help, new HelpVM(viewModel)},
                { PopupParams.LoadLevel, new LoadLevelVM(viewModel)},
                { PopupParams.GameSaved, new GameSavedVM(viewModel)},
                { PopupParams.CloseNotification, null},
            };

            if (viewModelsList.ContainsKey((PopupParams)parameter))
            {
                viewModel.SelectedViewModel = viewModelsList[(PopupParams)parameter];
            }
        }
    }
}