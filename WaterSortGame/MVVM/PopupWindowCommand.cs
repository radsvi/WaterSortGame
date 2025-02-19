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
    internal class ViewModelListPopup
    {
        public Func<ViewModelBase> InitializeType { get; set; }
        public Action ConfirmAction { get; set; }
        public ViewModelListPopup(Func<ViewModelBase> initializeType, Action confirmAction)
        {
            InitializeType = initializeType;
            ConfirmAction = confirmAction;
        }
    }
    internal class PopupWindowCommand : ICommand
    {
        public Dictionary<PopupParams, ViewModelListPopup> ViewModelsList { get; set; }
        private MainWindowVM viewModel;
        public PopupWindowCommand(MainWindowVM viewModel)
        {
            this.viewModel = viewModel;

            ViewModelsList = new Dictionary<PopupParams, ViewModelListPopup>
            {
                { PopupParams.NewLevel, new ViewModelListPopup(() => new NewLevelVM(viewModel), () => Execute(viewModel)) },
                { PopupParams.RestartLevel, new ViewModelListPopup(() => new RestartLevelVM(viewModel), () => Execute(viewModel)) },
                { PopupParams.LevelComplete, new ViewModelListPopup(() => new LevelCompleteVM(viewModel), () => Execute(viewModel)) },
                { PopupParams.Help, new ViewModelListPopup(() => new HelpVM(viewModel), () => Execute(viewModel)) },
                { PopupParams.LoadLevel, new ViewModelListPopup(() => new LoadLevelVM(viewModel), () => Execute(viewModel)) },
                { PopupParams.GameSaved, new ViewModelListPopup(() => new GameSavedNotificationVM(viewModel), () => Execute(viewModel)) },
                { PopupParams.SaveLevel, new ViewModelListPopup(() => new SaveLevelVM(viewModel), () => Execute(viewModel)) },

                //{ PopupParams.CloseNotification, () => null },
            };
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

            if (ViewModelsList.TryGetValue((PopupParams)parameter, out var output))
            {
                viewModel.SelectedViewModel = output.InitializeType();
            }
        }
    }
}