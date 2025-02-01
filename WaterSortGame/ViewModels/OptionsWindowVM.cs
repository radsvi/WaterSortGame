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
        public MainWindowVM MainWindowVM { get; set; }

        //public OptionsWindowVM(IWindowService windowService)
        public OptionsWindowVM(object mainWindowVM)
        {
            //this.windowService = windowService;
            MainWindowVM = (MainWindowVM)mainWindowVM;
            //this.windowService = mainWindowVM.WindowService;
            this.windowService = new WindowService();

        }

        public RelayCommand CloseOptionsWindowCommand => new RelayCommand(execute => CloseWindow());
        private void CloseWindow()
        {
            // ## predelat na MVVM model: https://www.youtube.com/watch?v=U7Qclpe2joo
            // ## C:\Users\svihe\Dropbox\Coding\C#\Testing\WpfTutorialsOther\How to Close Windows in MVVM\MainWindowViewModel.cs
            //System.Windows.Application.Current.Shutdown();

            windowService?.CloseWindow();
        }
    }
}
