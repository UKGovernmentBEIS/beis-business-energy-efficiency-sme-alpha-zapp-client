﻿using System;
using Caliburn.Micro;
using Notifications.Wpf;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.ViewModels;
using static RemindSME.Desktop.Properties.Resources;

namespace RemindSME.Desktop.Helpers
{
    public interface IReminderManager
    {
        bool HeatingOptIn { get; set; }
        void HandleNetworkCountChange(int count);
        void ShowHeatingNotificationIfOptedIn(string title, string message);
        void MaybeShowTimeDependentNotifications();
    }

    public class ReminderManager : IReminderManager
    {
        private const int LastOutThreshold = 3;

        private static readonly TimeSpan FirstLoginMinimumTime = new TimeSpan(06, 00, 00);
        private static readonly TimeSpan FirstLoginMaximumTime = new TimeSpan(11, 00, 00);
        private static readonly TimeSpan LastOutMinimumTime = new TimeSpan(17, 00, 00);

        private readonly IActionTracker actionTracker;
        private readonly INotificationManager notificationManager;

        private DateTime? mostRecentFirstLoginHeatingNotification;
        private DateTime? mostRecentLastOutNotification;

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

        public void MaybeShowTimeDependentNotifications()
        {
            MaybeShowFirstLoginHeatingNotification();
            MaybeShowLastOutNotification();
        }

        private void MaybeShowFirstLoginHeatingNotification()
        {
            if (!HeatingOptIn)
            {
                return;
            }

            var time = DateTime.Now.TimeOfDay;
            var notTooEarly = time >= FirstLoginMinimumTime;
            var earlyEnough = time <= FirstLoginMaximumTime;
            var noNotificationYetToday = mostRecentFirstLoginHeatingNotification?.Date != DateTime.Today;

            if (earlyEnough && notTooEarly && noNotificationYetToday)
            {
                ShowFirstLoginHeatingNotification();
            }
        }

        private void MaybeShowLastOutNotification()
        {
            var lateEnough = DateTime.Now.TimeOfDay >= LastOutMinimumTime;
            var fewEnoughPeople = networkCount <= LastOutThreshold;
            var noNotificationYetToday = mostRecentLastOutNotification?.Date != DateTime.Today;

            if (lateEnough && fewEnoughPeople && noNotificationYetToday)
            {
                ShowLastOutNotification();
            }
        }

        private void ShowFirstLoginHeatingNotification()
        {
            mostRecentFirstLoginHeatingNotification = DateTime.Now;
            ShowNotification(FirstLoginHeatingNotificationTitle, FirstLoginHeatingNotificationMessage);
        }

        private void ShowLastOutNotification()
        {
            const string title = "Staying a bit later?";
            const string message = "Don't forget to switch off the lights and heating if you're the last one out tonight!";
            mostRecentLastOutNotification = DateTime.Now;
            ShowNotification(title, message);
        }

        private void ShowNotification(string title, string message)
        {
            actionTracker.Log($"Displayed '{title}' notification.");

            var model = IoC.Get<NotificationViewModel>();
            model.Title = title;
            model.Message = message;
            notificationManager.Show(model, expirationTime: TimeSpan.FromHours(12),
                onClose: () => actionTracker.Log($"User dismissed '{title}' notification."));
        }
    }
}
