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
        public ViewModelListPopup(PopupParams key, ViewModelBase initializeType, Action confirmAction)
        {
            Key = key;
            SelectedViewModel = initializeType;
            ConfirmationAction = confirmAction;
        }
        public PopupParams Key { get; set; }
        public ViewModelBase SelectedViewModel { get; set; }
        public Action ConfirmationAction { get; set; }
    }
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

            var output = viewModel.PopupActions.Find(x => x.Key == (PopupParams)parameter);
            if (output != null)
            {
                viewModel.SelectedViewModel = output.SelectedViewModel;
            }
        }
    }
}