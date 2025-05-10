using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal class Notification
    {
        MainWindowVM MainWindowVM;
        const int closeDelayDefault = 2000; // in ms
        public CancellationTokenSource TokenSource { get; set; } = null;
        public Notification(MainWindowVM mainWindowVM)
        {
            MainWindowVM = mainWindowVM;
        }
        
        public void Show(string text, int closeDelay = closeDelayDefault)
        {
            MainWindowVM.QuickNotificationText = text;
            MainWindowVM.QuickNotificationVisibilityBool = true;

            TokenSource = new CancellationTokenSource();
            var token = TokenSource.Token;
            PopupNotification(token, closeDelay);
        }
        private async void PopupNotification(CancellationToken token, int closeDelay)
        {
            closeDelay = closeDelay / 100;
            for (int i = 0; i < closeDelay; i++)
            {
                await Task.Delay(100);
                if (token.IsCancellationRequested)
                {
                    break;
                }
            }

            ClosePopupWindow();
        }
        public void CloseNotification()
        {
            TokenSource?.Cancel();
        }
        private void ClosePopupWindow()
        {
            MainWindowVM.QuickNotificationVisibilityBool = false;
        }
    }
}
