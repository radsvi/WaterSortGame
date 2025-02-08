using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.ViewModels
{
    internal class HelpVM : ViewModelBase
    {
        public MainWindowVM MainWindowVM { get; set; }
        public HelpVM(object viewModel)
        {
            MainWindowVM = (MainWindowVM)viewModel;
        }
    }
}
