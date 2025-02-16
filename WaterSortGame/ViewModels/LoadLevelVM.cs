﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.ViewModels
{
    class LoadLevelVM : PopupScreenBase
    {
        public LoadLevelVM(object viewModel) : base(viewModel)
        {
            MainWindowVM = (MainWindowVM)viewModel;
            MainWindowVM.LoadLevelScreen();
        }
    }
}
