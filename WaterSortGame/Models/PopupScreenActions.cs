using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal class PopupScreenActions : ViewModelBase
    {
        public PopupScreenActions(PopupParams key, ViewModelBase initializeType, Action onShowingWindow, Action confirmAction)
        {
            Key = key;
            SelectedViewModel = initializeType;
            OnShowingWindow = onShowingWindow;
            ConfirmationAction = confirmAction;
        }
        public PopupParams Key { get; private set; }
        public ViewModelBase SelectedViewModel { get; private set; }
        public Action? OnShowingWindow { get; private set; }
        public Action ConfirmationAction { get; private set; }
        //private PopupParams key;
        //public PopupParams Key
        //{
        //    get { return key; }
        //    set
        //    {
        //        if (value != key)
        //        {
        //            key = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}
        //private ViewModelBase selectedViewModel;
        //public ViewModelBase SelectedViewModel
        //{
        //    get { return selectedViewModel; }
        //    set
        //    {
        //        if (value != selectedViewModel)
        //        {
        //            selectedViewModel = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}
        //private Action? onShowingWindow;
        //public Action? OnShowingWindow
        //{
        //    get { return onShowingWindow; }
        //    set
        //    {
        //        if (value != onShowingWindow)
        //        {
        //            onShowingWindow = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}
        //private Action confirmationAction;
        //public Action ConfirmationAction
        //{
        //    get { return confirmationAction; }
        //    set
        //    {
        //        if (value != confirmationAction)
        //        {
        //            confirmationAction = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}
    }
}
