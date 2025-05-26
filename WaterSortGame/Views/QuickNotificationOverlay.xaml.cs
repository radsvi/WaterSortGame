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
using WaterSortGame.Models;
using WaterSortGame.MVVM;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Views
{
    /// <summary>
    /// Interaction logic for QuickNotificationOverlay.xaml
    /// </summary>
    public partial class QuickNotificationOverlay : UserControl
    {
        MainWindowVM MainWindowVM;
        public QuickNotificationOverlay()
        {
            InitializeComponent();
        }
        internal QuickNotificationOverlay(MainWindowVM mainWindowVM, string notificationText, CancellationTokenSource tokenSource)
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;

            MainWindowVM = mainWindowVM;
            NotificationText = notificationText;
            NotificationDetails = new NotificationDetails(this, tokenSource);
        }
        public NotificationDetails NotificationDetails { get; private set; }
        public string NotificationText
        {
            get { return (string)GetValue(NotificationTextProperty); }
            set { SetValue(NotificationTextProperty, value); }
        }

        public static DependencyProperty NotificationTextProperty =
            DependencyProperty.Register("NotificationText", typeof(string), typeof(QuickNotificationOverlay));

        public RelayCommand CloseQuickNotificationCommandInternal => new RelayCommand(notificationDetails => MainWindowVM.Notification.CloseNotification(notificationDetails));
    }
}
