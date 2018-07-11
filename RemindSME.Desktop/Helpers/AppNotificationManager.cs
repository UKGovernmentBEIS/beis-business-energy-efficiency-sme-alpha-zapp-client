using System;
using System.Threading.Tasks;
using Notifications.Wpf;

namespace RemindSME.Desktop.Helpers
{
    public class AppNotificationManager : INotificationManager
    {
        private readonly NotificationManager notificationManager;
        private readonly IUserNotificationStateHelper userNotificationStateHelper;

        public AppNotificationManager(NotificationManager notificationManager, IUserNotificationStateHelper userNotificationStateHelper)
        {
            this.notificationManager = notificationManager;
            this.userNotificationStateHelper = userNotificationStateHelper;
        }

        public async void Show(object content, string areaName = "", TimeSpan? expirationTime = null, Action onClick = null, Action onClose = null)
        {
            while (!userNotificationStateHelper.AcceptingNotifications())
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            notificationManager.Show(content, areaName, expirationTime, onClick, onClose);
        }
    }
}
