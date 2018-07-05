﻿using System;
using System.Windows.Threading;
using Caliburn.Micro;
using Notifications.Wpf;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Events;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.ViewModels;

namespace RemindSME.Desktop.Services
{
    public class ReminderService : IService, IHandle<HeatingNotificationEvent>, IHandle<NetworkCountChangeEvent>
    {
        private const int LastOutThreshold = 3;

        private static readonly TimeSpan FirstLoginMinimumTime = new TimeSpan(06, 00, 00);
        private static readonly TimeSpan FirstLoginMaximumTime = new TimeSpan(11, 00, 00);
        private static readonly TimeSpan LastToLeaveMinimumTime = new TimeSpan(17, 00, 00);

        private readonly ILog log;
        private readonly IAppWindowManager appWindowManager;
        private readonly IEventAggregator eventAggregator;
        private readonly INotificationManager notificationManager;
        private readonly ISettings settings;
        private readonly DispatcherTimer timer;

        private int? networkCount;

        public ReminderService(
            ILog log,
            INotificationManager notificationManager,
            IAppWindowManager appWindowManager,
            IEventAggregator eventAggregator,
            ISettings settings,
            DispatcherTimer timer)
        {
            this.log = log;
            this.notificationManager = notificationManager;
            this.appWindowManager = appWindowManager;
            this.eventAggregator = eventAggregator;
            this.settings = settings;
            this.timer = timer;
        }

        public void Initialize()
        {
            eventAggregator.Subscribe(this);

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void Handle(HeatingNotificationEvent e)
        {
            if (ShouldShowHeatingNotification())
            {
                ShowHeatingNotification(e.Title, e.Message);
            }
        }

        public void Handle(NetworkCountChangeEvent e)
        {
            networkCount = e.Count;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (appWindowManager.AnyAppWindowIsOpen())
            {
                // Suppress all timed reminders while app windows are open.
                // This avoids immediately showing notifications to new or newly opted-in users.
                return;
            }

            if (ShouldShowLastToLeaveNotification())
            {
                ShowLastToLeaveNotification();
            }

            if (ShouldShowFirstLoginHeatingNotification())
            {
                ShowFirstLoginHeatingNotification();
            }
        }

        private bool ShouldShowHeatingNotification()
        {
            return settings.HeatingOptIn;
        }

        private void ShowHeatingNotification(string title, string message)
        {
            ShowNotification(title ?? Resources.Notification_HeatingDefault_Title, message ?? Resources.Notification_HeatingDefault_Message);
        }

        private bool ShouldShowFirstLoginHeatingNotification()
        {
            var time = DateTime.Now.TimeOfDay;
            return settings.HeatingOptIn && // Opted in.
                   time >= FirstLoginMinimumTime && // Not too early.
                   time <= FirstLoginMaximumTime && // Early enough.
                   settings.MostRecentFirstLoginNotification.Date != DateTime.Today; // No notification yet today.
        }

        private void ShowFirstLoginHeatingNotification()
        {
            settings.MostRecentFirstLoginNotification = DateTime.Now;
            ShowNotification(Resources.Notification_HeatingFirstLogin_Title, Resources.Notification_HeatingFirstLogin_Message);
        }

        private bool ShouldShowLastToLeaveNotification()
        {
            return DateTime.Now.TimeOfDay >= LastToLeaveMinimumTime && // Late enough.
                   networkCount.HasValue && // Is on a counted network.
                   networkCount.Value <= LastOutThreshold && // Few enough people.
                   settings.MostRecentLastToLeaveNotification.Date != DateTime.Today; // No notification yet today.
        }

        private void ShowLastToLeaveNotification()
        {
            settings.MostRecentLastToLeaveNotification = DateTime.Now;
            ShowNotification(Resources.Notification_LastOut_Title, Resources.Notification_LastOut_Message);
        }

        private void ShowNotification(string title, string message)
        {
            log.Info($"Displayed '{title}' notification.");

            var model = IoC.Get<ReminderViewModel>();
            model.Title = title;
            model.Message = message;
            notificationManager.Show(model, expirationTime: TimeSpan.FromMinutes(15),
                onClose: () => log.Info($"User dismissed '{title}' notification."));
        }
    }
}
