using System;
using Caliburn.Micro;
using Notifications.Wpf;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.ViewModels;

namespace RemindSME.Desktop.Helpers
{
    public interface IReminderManager
    {
        bool HeatingOptIn { get; set; }
        void HandleNetworkCountChange(int count);
        void MaybeShowLastManNotification();
        void ShowHeatingNotificationIfOptedIn(string title, string message);
    }

    public class ReminderManager : IReminderManager
    {
        private const int LastManThreshold = 3;
        private static readonly TimeSpan LastManMinimumTime = new TimeSpan(17, 00, 00);

        private readonly IActionTracker actionTracker;
        private readonly INotificationManager notificationManager;

        // Don't show last man notification twice on the same day.
        private DateTime? mostRecentLastManNotification;

        private int networkCount;

        public ReminderManager(IActionTracker actionTracker, INotificationManager notificationManager)
        {
            this.actionTracker = actionTracker;
            this.notificationManager = notificationManager;
        }

        public bool HeatingOptIn
        {
            get => Settings.Default.HeatingOptIn;
            set
            {
                Settings.Default.HeatingOptIn = value;
                Settings.Default.Save();
            }
        }

        public void HandleNetworkCountChange(int count)
        {
            networkCount = count;
        }

        public void MaybeShowLastManNotification()
        {
            var lateEnough = DateTime.Now.TimeOfDay >= LastManMinimumTime;
            var fewEnoughPeople = networkCount <= LastManThreshold;
            var noNotificationYetToday = !HasSeenLastManNotificationToday();

            if (lateEnough && fewEnoughPeople && noNotificationYetToday)
            {
                ShowLastManNotification();
            }
        }

        public void ShowHeatingNotificationIfOptedIn(string title, string message)
        {
            if (!HeatingOptIn)
            {
                return;
            }
            title = title ?? "It's hot!";
            message = message ?? "Have you checked your AC settings?";
            ShowNotification(title, message);
        }

        private void ShowLastManNotification()
        {
            const string title = "Staying a bit later?";
            const string message = "Don't forget to switch off the lights and heating if you're the last one out tonight!";
            mostRecentLastManNotification = DateTime.Now;
            ShowNotification(title, message);
        }

        private bool HasSeenLastManNotificationToday()
        {
            return mostRecentLastManNotification?.Date == DateTime.Today;
        }

        private void ShowNotification(string title, string message)
        {
            actionTracker.Log($"Displayed '{title}' notification.");

            var model = IoC.Get<NotificationViewModel>();
            model.Title = title;
            model.Message = message;
            notificationManager.Show(model, expirationTime: TimeSpan.FromHours(2),
                onClose: () => actionTracker.Log($"User dismissed '{title}' notification."));
        }
    }
}
