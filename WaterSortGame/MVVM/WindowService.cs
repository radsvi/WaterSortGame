using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaterSortGame.Views;
using WaterSortGame.ViewModels;

namespace WaterSortGame.MVVM
{
    public class WindowService : IWindowService
    {
        public void OpenWindow(object mainWindowVM)
        {
            // Create an instance of the new window
            //new OptionsWindow().DataContext = new OptionsWindowVM();
            var window = new OptionsWindow();
            var viewModel = new OptionsWindowVM(mainWindowVM);
            //window.DataContext = viewModel;
            window.DataContext = mainWindowVM;

            // Show the new window
            window.ShowDialog();
        }
        public void CloseWindow()
        {
            // Get a reference to the current window
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

            // Close the window
            if (window is not null)
            {
                window.Close();
            }
        }
    }
}
