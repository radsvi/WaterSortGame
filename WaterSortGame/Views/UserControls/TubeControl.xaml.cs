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
    public partial class FlaskControl : UserControl
    {
        //internal TubeControl(MainWindowVM mainWindowVM, Tube tubeItem)
        //{
        //    InitializeComponent();
        //    (this.Content as FrameworkElement).DataContext = this;

        //    MainWindowVM = mainWindowVM;
        //    TubeItem = tubeItem;
        //}
        internal FlaskControl(MainWindowVM mainWindowVM, int tubeId, LiquidColor[] liquidColors)
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;

            MainWindowVM = mainWindowVM;
            TubeId = tubeId;
            LiquidColors = liquidColors;
        }
        private MainWindowVM MainWindowVM { get; }
        public int TubeId { get; }
        internal LiquidColor[] LiquidColors
        {
            get { return (LiquidColor[])GetValue(LiquidColorsProperty); }
            set { SetValue(LiquidColorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TubeItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LiquidColorsProperty =
            DependencyProperty.Register("LiquidColors", typeof(LiquidColor[]), typeof(FlaskControl));



        //public int TubeId
        //{
        //    get { return (int)GetValue(TubeIdProperty); }
        //    set { SetValue(TubeIdProperty, value); }
        //}
        //// Using a DependencyProperty as the backing store for TubeId.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty TubeIdProperty =
        //    DependencyProperty.Register("TubeId", typeof(int), typeof(int));




        public RelayCommand SelectTubeCommandInternal => new RelayCommand(execute => MainWindowVM.OnTubeButtonClick(execute));
        
    }
}
