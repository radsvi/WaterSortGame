using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.Models;
using WaterSortGame.MVVM;
using WaterSortGame.Properties;

namespace WaterSortGame.ViewModels
{
    class LoadLevelVM : PopupScreenBase
    {
        public LoadLevelVM(object viewModel) : base(viewModel)
        {
            MainWindowVM = (MainWindowVM)viewModel;
            //MainWindowVM.LoadLevelScreen();
        }

        
    }
}
