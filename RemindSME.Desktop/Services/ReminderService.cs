using System;
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
    public interface IReminderService : IService { }

    public class ReminderService : IReminderService, IHandle<HeatingNotificationEvent>, IHandle<NetworkCountChangeEvent>
    {
        private const int LastOutThreshold = 3;

        private static readonly TimeSpan FirstLoginMinimumTime = new TimeSpan(06, 00, 00);
        private static readonly TimeSpan FirstLoginMaximumTime = new TimeSpan(11, 00, 00);
        private static readonly TimeSpan LastToLeaveMinimumTime = new TimeSpan(17, 00, 00);

        private readonly IActionTracker actionTracker;
        private readonly IAppWindowManager appWindowManager;
        private readonly IEventAggregator eventAggregator;
        private readonly INotificationManager notificationManager;
        private readonly ISettings settings;
        private readonly DispatcherTimer timer;

        private DateTime? mostRecentFirstLoginHeatingNotification;
        private DateTime? mostRecentLastOutNotification;

        private int networkCount = int.MaxValue;

        public ReminderService(
            IActionTracker actionTracker,
            INotificationManager notificationManager,
            IAppWindowManager appWindowManager,
            IEventAggregator eventAggregator,
            ISettings settings,
            DispatcherTimer timer)
        {
            this.actionTracker = actionTracker;
            this.notificationManager = notificationManager;
            this.appWindowManager = appWindowManager;
            this.eventAggregator = eventAggregator;
            this.settings = settings;
            this.timer = timer;
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

        public void Initialize()
        {
            eventAggregator.Subscribe(this);

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
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
                   mostRecentFirstLoginHeatingNotification?.Date != DateTime.Today; // No notification yet today.
        }

        private void ShowFirstLoginHeatingNotification()
        {
            mostRecentFirstLoginHeatingNotification = DateTime.Now;
            ShowNotification(Resources.Notification_HeatingFirstLogin_Title, Resources.Notification_HeatingFirstLogin_Message);
        }

        private bool ShouldShowLastToLeaveNotification()
        {
            return DateTime.Now.TimeOfDay >= LastToLeaveMinimumTime && // Late enough.
                   networkCount <= LastOutThreshold && // Few enough people.
                   mostRecentLastOutNotification?.Date != DateTime.Today; // No notification yet today.
        }

        private void ShowLastToLeaveNotification()
        {
            mostRecentLastOutNotification = DateTime.Now;
            ShowNotification(Resources.Notification_LastOut_Title, Resources.Notification_LastOut_Message);
        }

        private void ShowNotification(string title, string message)
        {
            actionTracker.Log($"Displayed '{title}' notification.");

            var model = IoC.Get<ReminderViewModel>();
            model.Title = title;
            model.Message = message;
            notificationManager.Show(model, expirationTime: TimeSpan.FromMinutes(15),
                onClose: () => actionTracker.Log($"User dismissed '{title}' notification."));
        }
    }
}
