using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WaterSortGame.Models;
using WaterSortGame.ViewModels;
using WaterSortGame.Enums;

namespace WaterSortGame.MVVM
{
    internal class PopupScreenCommand : ICommand
    {
        private MainWindowVM mainWindowVM;
        public PopupScreenCommand(MainWindowVM viewModel)
        {
            this.mainWindowVM = viewModel;
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
                mainWindowVM.SelectedViewModel = null;
                return;
            }

            var output = mainWindowVM.PopupActions.Where(x => x.Key == (PopupParams)parameter);
            //var output = Array.Find(viewModel.PopupActions, x => x.Key == (PopupParams)parameter);
            if (output != null && output.Count() == 1)
            {
                mainWindowVM.SelectedViewModel = output.ElementAt(0).SelectedViewModel;
                output.ElementAt(0).OnShowingWindow?.Invoke();
            }
        }
    }
}