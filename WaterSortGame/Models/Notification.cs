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
    //internal class NotificationDetails
    //{
    //    public NotificationDetails(QuickNotificationOverlay refVisualElement, CancellationToken token)
    //    {
    //        RefVisualElement = refVisualElement;
    //        Token = token;
    //    }
    //    public CancellationToken Token { get; private set; }
    //    public QuickNotificationOverlay RefVisualElement { get; private set; }
    //}
    //internal class NotificationsList
    //{
    //    public List<NotificationDetails> Notifications { get; private set; }
    //    public Panel NotificationBox { get; private set; }
    //    public NotificationsList(Panel notificationBoxVisualElement)
    //    {
    //        NotificationBox = notificationBoxVisualElement;
    //    }
    //    public void Add(QuickNotificationOverlay notificationControl, CancellationToken token)
    //    {
    //        NotificationBox.Children.Add(notificationControl);
    //        var reference = new NotificationDetails(notificationControl, token);
    //        Notifications.Add(reference);
    //    }
    //}
    internal class Notification
    {
        MainWindowVM MainWindowVM;
        const int closeDelayDefault = 2000; // in ms
        [Obsolete]public int Counter { get; set; } = 0;
        private bool DisplayDebugMessages { get; set; } = true;
        public CancellationTokenSource TokenSource { get; set; } = null;
        public Panel NotificationBox { get; private set; }
        //public NotificationsList NotificationList { get; private set; }
        public Dictionary<CancellationToken, QuickNotificationOverlay> NotificationsDictionary { get; private set; } = [];
        public Notification(MainWindowVM mainWindowVM)
        {
            MainWindowVM = mainWindowVM;
            //NotificationList = new NotificationsList(MainWindowVM.MainWindow.NotificationBox);
            NotificationBox = mainWindowVM.MainWindow.NotificationBox;
        }
        public void Show(string text) => Show(text, MessageType.Information);
        public void Show(string text, int closeDelay = closeDelayDefault) => Show(text, MessageType.Information, closeDelayDefault);
        public void Show(string text, MessageType messageType, int closeDelay = closeDelayDefault)
        {
            Debug.WriteLine("[Notification: ]" + text);
            if (messageType == MessageType.Debug && DisplayDebugMessages is false)
            {
                return;
            }

            MainWindowVM.QuickNotificationText = text;
            MainWindowVM.QuickNotificationVisibilityBool = true;

            TokenSource = new CancellationTokenSource();
            var token = TokenSource.Token;

            //var notificationControl = new QuickNotificationOverlay() { NotificationText = $"{{{Counter++}}}" + text, Token = token };
            var notificationControl = new QuickNotificationOverlay(MainWindowVM, $"{{{Counter++}}}" + text, token);
            //NotificationList.Add(notificationControl, token);
            NotificationBox.Children.Add(notificationControl);
            NotificationsDictionary.Add(token, notificationControl);

            PopupNotification(token, closeDelay);
        }
        private async void PopupNotification(CancellationToken token, int closeDelay)
        {
            //await Task.Delay(closeDelayDefault, token);

            closeDelay = closeDelay / 100;
            for (int i = 0; i < closeDelay; i++)
            {
                await Task.Delay(100);
                if (token.IsCancellationRequested)
                {
                    break;
                }
            }

            ClosePopupWindow(token);
        }
        public void CloseNotification(object tokenObject)
        {
            if (tokenObject.GetType() != typeof(CancellationToken))
            {
                return;
            }
            CancellationToken token = (CancellationToken)tokenObject;

            

            TokenSource?.Cancel();
        }
        private void ClosePopupWindow(CancellationToken token)
        {
            if (NotificationsDictionary.ContainsKey(token))
            {
                NotificationBox.Children.Remove(NotificationsDictionary[token]);
                NotificationsDictionary.Remove(token);
            }
        }
    }
}
