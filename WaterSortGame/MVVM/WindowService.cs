using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaterSortGame.Views;
using WaterSortGame.ViewModels;
using System.Runtime.CompilerServices;

namespace WaterSortGame.MVVM
{
    public class WindowService : IWindowService
    {
        public void OpenOptionsWindow(object sender)
        {
            var window = new OptionsWindow();
            window.DataContext = sender;
            MainWindowVM mainWindowVM = (MainWindowVM)sender;
            MainWindow mainWindow = mainWindowVM.MainWindow;

            window.Top = (mainWindow.Top + 30);
            window.Left = (mainWindow.Left + (mainWindow.Width - mainWindowVM.OptionsWindowWidth) / 2);

            window.ShowDialog();
        }
        public void OpenLevelCompleteWindow(object sender)
        {
            var window = new LevelCompleteWindow();
            window.DataContext = sender;
            MainWindowVM mainWindowVM = (MainWindowVM)sender;
            MainWindow mainWindow = mainWindowVM.MainWindow;

            window.Top = (mainWindow.Top + 90);
            window.Left = (mainWindow.Left + (mainWindow.Width - window.Width) / 2);
            
            window.ShowDialog();
        }

        public void CloseWindow()
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

            if (window is not null)
            {
                window.Close();
            }
        }

    }
}
