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
using WaterSortGame.MVVM;

namespace WaterSortGame.Views.UserControls
{
    /// <summary>
    /// Interaction logic for NumericTextBox.xaml
    /// </summary>
    partial class NumericTextBox : UserControl
    {
        public NumericTextBox()
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }
        [Range(1, 100)]
        public int NumTextBoxText
        {
            get { return (int)GetValue(NumTextBoxTextProperty); }
            set
            {
                if (value >= 1 && value <= 100)
                {
                    SetValue(NumTextBoxTextProperty, value);
                }
                else if (value < 1)
                {
                    SetValue(NumTextBoxTextProperty, 1);
                }
                else if (value > 100)
                {
                    SetValue(NumTextBoxTextProperty, 100);
                }
                else
                {
                    SetValue(NumTextBoxTextProperty, 1);
                }
            }
        }

        public static readonly DependencyProperty NumTextBoxTextProperty =
            DependencyProperty.Register("NumTextBoxText", typeof(int), typeof(NumericTextBox), new PropertyMetadata(0));

        RelayCommand NumericTextBoxUpInternalCommand => new RelayCommand(execute => NumTextBoxText++);
        RelayCommand NumericTextBoxDownInternalCommand => new RelayCommand(execute => NumTextBoxText--);
    }
}
