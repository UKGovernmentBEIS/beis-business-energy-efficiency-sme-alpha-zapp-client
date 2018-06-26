using System;
using Caliburn.Micro;
using Notifications.Wpf;
using RemindSME.Desktop.ViewModels;

namespace RemindSME.Desktop.Helpers
{
    public interface IReminderManager
    {
        void UpdateNetworkCount(int count);
    }

    public class ReminderManager : IReminderManager
    {
        private readonly INotificationManager notificationManager;

        public ReminderManager(INotificationManager notificationManager)
        {
            this.notificationManager = notificationManager;
        }

        public void UpdateNetworkCount(int count)
        {
            if (count <= 3)
            {
                ShowNotification(
                    "Staying a bit later?", 
                    "Don't forget to switch out the lights if you're the last one out tonight.");
            }
        }

        public void ShowNotification(string title, string message)
        {
            var model = IoC.Get<NotificationViewModel>();
            model.Title = title;
            model.Message = message;
            notificationManager.Show(model, expirationTime: TimeSpan.FromHours(2));
        }
    }
}
