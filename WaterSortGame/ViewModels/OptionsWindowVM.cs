using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.MVVM;

namespace WaterSortGame.ViewModels
{
    internal class OptionsWindowVM : ViewModelBase
    {
        public int WindowHeight { get; set; } = 450;
        public int WindowWidth { get; set; } = 800;

        private IWindowService windowService;

        //public OptionsWindowVM(IWindowService windowService)
        public OptionsWindowVM()
        {
            //this.windowService = windowService;
        }
    }
}
