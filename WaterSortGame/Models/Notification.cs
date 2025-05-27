using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WaterSortGame.ViewModels;
using WaterSortGame.Views;

namespace WaterSortGame.Models
{
    internal enum MessageType
    {
        Information,
        Debug
    }
    public class NotificationDetails
    {
        public NotificationDetails(QuickNotificationOverlay refVisualElement, CancellationTokenSource tokenSource)
        {
            RefVisualElement = refVisualElement;
            TokenSource = tokenSource;
        }
        public CancellationTokenSource TokenSource { get; private set; }
        public QuickNotificationOverlay RefVisualElement { get; private set; }
    }
    internal class Notification
    {
        MainWindowVM MainWindowVM;
        const int closeDelayDefault = 2000; // in ms
        private bool DisplayDebugMessages { get; set; } = true;
        //public CancellationTokenSource TokenSource { get; set; } = null;
        public Panel NotificationBox { get; private set; }
        //public NotificationsList NotificationList { get; private set; }
        public Notification(MainWindowVM mainWindowVM)
        {
            MainWindowVM = mainWindowVM;
            //NotificationList = new NotificationsList(MainWindowVM.MainWindow.NotificationBox);
            NotificationBox = mainWindowVM.MainWindow.NotificationBox;
        }
        public void Show(string text) => Show(text, MessageType.Information);
        public void Show(string text, int closeDelay = closeDelayDefault) => Show(text, MessageType.Information, closeDelay);
        public void Show(string text, MessageType messageType, int closeDelay = closeDelayDefault)
        {
            Debug.WriteLine("[Notification: ]" + text);
            if (messageType == MessageType.Debug && DisplayDebugMessages is false)
            {
                return;
            }

            var tokenSource = new CancellationTokenSource();
            var notificationControl = new QuickNotificationOverlay(MainWindowVM, text, tokenSource);

            //NotificationBox.Children.Add(notificationControl);
            NotificationBox.Children.Insert(0, notificationControl);

            PopupNotification(notificationControl, closeDelay);
        }
        private async void PopupNotification(QuickNotificationOverlay notificationControl, int closeDelay)
        {
            //await Task.Delay(closeDelayDefault, token);

            closeDelay = closeDelay / 100;
            for (int i = 0; i < closeDelay; i++)
            {
                await Task.Delay(100);
                if (notificationControl.NotificationDetails.TokenSource.Token.IsCancellationRequested)
                {
                    break;
                }
            }

            ClosePopupWindow(notificationControl.NotificationDetails);
        }
        public void CloseNotification(object notificationDetailsGenericObject)
        {
            if (notificationDetailsGenericObject.GetType() != typeof(NotificationDetails))
            {
                return;
            }
            NotificationDetails notificationDetails = (NotificationDetails)notificationDetailsGenericObject;
            notificationDetails.TokenSource.Cancel();
        }
        private void ClosePopupWindow(NotificationDetails notificationDetails)
        {
            NotificationBox.Children.Remove(notificationDetails.RefVisualElement);
        }
    }
}
