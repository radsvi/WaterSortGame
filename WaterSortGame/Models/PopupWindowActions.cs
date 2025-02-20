using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal class PopupWindowActions
    {
        public PopupWindowActions(PopupParams key, ViewModelBase initializeType, Action onShowingWindow, Action confirmAction)
        {
            Key = key;
            SelectedViewModel = initializeType;
            OnShowingWindow = onShowingWindow;
            ConfirmationAction = confirmAction;
        }
        public PopupParams Key { get; set; }
        public ViewModelBase SelectedViewModel { get; set; }
        public Action? OnShowingWindow { get; set; }
        public Action ConfirmationAction { get; set; }
    }
}
