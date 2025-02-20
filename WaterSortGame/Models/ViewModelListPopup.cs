using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
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
}
