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
    /// Interaction logic for QuickNotificationOverlay.xaml
    /// </summary>
    public partial class QuickNotificationOverlay : UserControl
    {
        public QuickNotificationOverlay()
        {
            InitializeComponent();
        }


        public string NotificationText
        {
            get { return (string)GetValue(NotificationTextProperty); }
            set { SetValue(NotificationTextProperty, value); }
        }

        public static DependencyProperty NotificationTextProperty =
            DependencyProperty.Register("NotificationText", typeof(string), typeof(QuickNotificationOverlay));


    }
}
