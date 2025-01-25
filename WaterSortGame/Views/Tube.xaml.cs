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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WaterSortGame.Views
{
    /// <summary>
    /// Interaction logic for Tube.xaml
    /// </summary>
    public partial class Tube : UserControl
    {
        public Tube()
        {
            // binding for user control: https://www.youtube.com/watch?v=se75uhsg3IA
            InitializeComponent();
            this.DataContext = this;
        }

        public string FourthLayer { get; set; }
        public string ThirdLayer { get; set; }
        public string SecondLayer { get; set; }
        public string FirstLayer { get; set; }

        //public static DependencyProperty ProtocolNumberProperty =
        //   DependencyProperty.Register("BoundText", typeof(int), typeof(Tube));
    }
}
