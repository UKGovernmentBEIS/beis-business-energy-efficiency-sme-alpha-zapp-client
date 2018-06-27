﻿using System;
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
        void ShowHeatingNotification();
    }

    public class ReminderManager : IReminderManager
    {
        private const int LastManThreshold = 3;
        private static readonly TimeSpan LastManMinimumTime = new TimeSpan(17, 00, 00);

        private readonly INotificationManager notificationManager;

        // Don't show last man notification twice on the same day.
        private DateTime? mostRecentLastManNotification;

        private int networkCount;

        public ReminderManager(INotificationManager notificationManager)
        {
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

        private void ShowLastManNotification()
        {
            const string title = "Staying a bit later?";
            const string message = "Don't forget to switch off the lights and heating if you're the last one out tonight!";
            mostRecentLastManNotification = DateTime.Now;
            ShowNotification(title, message);
        }

        public void ShowHeatingNotification()
        {
            if (!HeatingOptIn)
            {
                return;
            }
            ShowNotification("It's hot!", "Have you checked your AC settings?");
        }

        private bool HasSeenLastManNotificationToday()
        {
            return mostRecentLastManNotification?.Date == DateTime.Today;
        }

        private void ShowNotification(string title, string message)
        {
            var model = IoC.Get<NotificationViewModel>();
            model.Title = title;
            model.Message = message;
            notificationManager.Show(model, expirationTime: TimeSpan.FromHours(2));
        }
    }
}
