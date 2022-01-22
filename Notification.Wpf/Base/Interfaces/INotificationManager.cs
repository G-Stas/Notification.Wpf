using System;
using Notification.Wpf.Controls;

namespace Notification.Wpf
{
    /// <summary> Notification manager for popup messages </summary>
    public interface INotificationManager
    {
        void ChangeNotificationAreaPosition(NotificationPosition newPosition);
        
        public void ShowNotification(object content, string key, bool displayCloseButton, TimeSpan? expirationTime = null);

        public void CloseNotification(string key);
    }
}