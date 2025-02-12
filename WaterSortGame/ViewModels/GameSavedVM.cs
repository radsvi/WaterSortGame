using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.ViewModels
{
    class GameSavedVM : ViewModelBase
    {
        public MainWindowVM MainWindowVM { get; set; }
        public GameSavedVM(object viewModel)
        {
            MainWindowVM = (MainWindowVM)viewModel;
        }
    }
}
