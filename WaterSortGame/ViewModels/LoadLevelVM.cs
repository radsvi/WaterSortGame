using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.ViewModels
{
    class LoadLevelVM : ViewModelBase
    {
        public MainWindowVM MainWindowVM { get; set; }
        public LoadLevelVM(object viewModel)
        {
            MainWindowVM = (MainWindowVM)viewModel;
        }
    }
}
