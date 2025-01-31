using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WaterSortGame.MVVM;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Views
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();

            //var ow = new OptionsWindowVM((IWindowService)mainWindowDataContext);
            //DataContext = ow;



            //Top = (SystemParameters.WorkArea.Height - ow.WindowHeight) / 2 + 50;
            //Left = (SystemParameters.WorkArea.Width - ow.WindowWidth) / 2;

            //MainWindowVM dtc = mainWindowDataContext as MainWindowVM;
            //Top = (SystemParameters.WorkArea.Height - ow.WindowHeight) / 2 + 50;
            //Left = (SystemParameters.WorkArea.Width - ow.WindowWidth) / 2;
        }
    }
}
