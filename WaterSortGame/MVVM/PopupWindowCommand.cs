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

            //var output = viewModel.PopupActions.Find(x => x.Key == (PopupParams)parameter);
            var output = Array.Find(viewModel.PopupActions, x => x.Key == (PopupParams)parameter);
            if (output != null)
            {
                viewModel.SelectedViewModel = output.SelectedViewModel;
                output.OnShowingWindow?.Invoke();
            }
        }
    }
}