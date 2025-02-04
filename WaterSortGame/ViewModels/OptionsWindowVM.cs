using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.MVVM;
using WaterSortGame.Properties;

namespace WaterSortGame.ViewModels
{
    internal class OptionsWindowVM : ViewModelBase
    {
        private int optionsWindowHeight = Settings.Default.OptionsWindowHeight;
        public int OptionsWindowHeight
        {
            get { return optionsWindowHeight; }
            set
            {
                optionsWindowHeight = value;
                Settings.Default.OptionsWindowHeight = value;
                Settings.Default.Save();
            }
        }
        private int optionsWindowWidth = Settings.Default.OptionsWindowWidth;
        public int OptionsWindowWidth
        {
            get { return optionsWindowWidth; }
            set
            {
                optionsWindowWidth = value;
                Settings.Default.OptionsWindowWidth = value;
                Settings.Default.Save();
            }
        }

        private IWindowService windowService;
        public MainWindowVM MainWindowVM { get; set; }

        //public OptionsWindowVM(IWindowService windowService)
        public OptionsWindowVM(object mainWindowVM)
        {
            //this.windowService = windowService;
            MainWindowVM = (MainWindowVM)mainWindowVM;
            //this.windowService = mainWindowVM.WindowService;
            this.windowService = new WindowService();

        }

        
    }
}
