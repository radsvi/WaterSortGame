using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WaterSortGame.Models;
using WaterSortGame.MVVM;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Views.UserControls
{
    /// <summary>
    /// Interaction logic for TubeControl.xaml
    /// </summary>
    public partial class TubeControl : UserControl
    {
        internal TubeControl(MainWindowVM mainWindowVM, Tube tubeItem)
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;

            MainWindowVM = mainWindowVM;
            TubeItem = tubeItem;
        }
        private MainWindowVM MainWindowVM { get; set; }
        internal Tube TubeItem
        {
            get { return (Tube)GetValue(TubeItemProperty); }
            set { SetValue(TubeItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TubeItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TubeItemProperty =
            DependencyProperty.Register("TubeItem", typeof(Tube), typeof(TubeControl));

        public RelayCommand SelectTubeCommandInternal => new RelayCommand(execute => MainWindowVM.OnClickingTube(execute));
        
    }
}
