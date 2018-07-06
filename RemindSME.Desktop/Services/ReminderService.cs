using System;
using System.Windows.Threading;
using Caliburn.Micro;
using Notifications.Wpf;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Events;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.ViewModels;
using Action = System.Action;

namespace RemindSME.Desktop.Services
{
    public class ReminderService : IService, IHandle<HeatingNotificationEvent>, IHandle<NetworkCountChangeEvent>
    {
        private const int FirstInThreshold = 3;
        private const int LastToLeaveThreshold = 3;

        private static readonly TimeSpan FirstLoginMinimumTime = new TimeSpan(06, 00, 00);
        private static readonly TimeSpan FirstLoginMaximumTime = new TimeSpan(11, 00, 00);
        private static readonly TimeSpan LastToLeaveMinimumTime = new TimeSpan(17, 00, 00);

        private static readonly TimeSpan LastToLeaveSnoozePeriod = TimeSpan.FromMinutes(30);

        private readonly IAppWindowManager appWindowManager;
        private readonly IEventAggregator eventAggregator;
        private readonly ILog log;
        private readonly INotificationManager notificationManager;
        private readonly ISettings settings;
        private readonly DispatcherTimer timer;

        private bool isShowingFirstLoginNotification;
        private bool isShowingLastToLeaveNotification;

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

            if (ShouldShowFirstLoginNotification())
            {
                ShowFirstLoginNotification();
            }
        }

        private bool ShouldShowHeatingNotification()
        {
            return settings.HeatingOptIn;
        }

        private void ShowHeatingNotification(string title, string message)
        {
            ShowNotification(
                title ?? Resources.Notification_HeatingDefault_Title,
                message ?? Resources.Notification_HeatingDefault_Message);
        }

        private bool ShouldShowFirstLoginNotification()
        {
            if (isShowingFirstLoginNotification)
            {
                return false;
            }

            var time = DateTime.Now.TimeOfDay;
            return settings.HeatingOptIn && // Opted in.
                   time >= FirstLoginMinimumTime && // Not too early.
                   time <= FirstLoginMaximumTime && // Early enough.
                   networkCount.HasValue && networkCount.Value <= FirstInThreshold && // Few enough people.
                   settings.MostRecentFirstLoginNotification.Date != DateTime.Today; // No notification yet today.
        }

        private void ShowFirstLoginNotification()
        {
            isShowingFirstLoginNotification = true;
            ShowNotification(
                Resources.Notification_HeatingFirstLogin_Title,
                Resources.Notification_HeatingFirstLogin_Message,
                () => isShowingFirstLoginNotification = false,
                new ReminderViewModel.Button("Done!", FirstLoginNotification_OK));
        }

        private void FirstLoginNotification_OK()
        {
            settings.MostRecentFirstLoginNotification = DateTime.Now;
        }

        private bool ShouldShowLastToLeaveNotification()
        {
            if (isShowingLastToLeaveNotification)
            {
                return false;
            }

            var now = DateTime.Now;
            return now.TimeOfDay >= LastToLeaveMinimumTime && // Late enough.
                   now >= settings.LastToLeaveNotificationSnoozeTime && // After any existing snooze.
                   networkCount.HasValue && networkCount.Value <= LastToLeaveThreshold && // Few enough people.
                   settings.MostRecentLastToLeaveNotification.Date != DateTime.Today; // Has not dismissed today.
        }

        private void ShowLastToLeaveNotification()
        {
            isShowingLastToLeaveNotification = true;
            ShowNotification(
                Resources.Notification_LastToLeave_Title,
                Resources.Notification_LastToLeave_Message,
                () => isShowingLastToLeaveNotification = false,
                new ReminderViewModel.Button("OK", LastToLeaveNotification_OK),
                new ReminderViewModel.Button("Snooze", LastToLeaveNotification_Snooze));
        }

        private void LastToLeaveNotification_OK()
        {
            settings.MostRecentLastToLeaveNotification = DateTime.Now;
        }

        private void LastToLeaveNotification_Snooze()
        {
            settings.LastToLeaveNotificationSnoozeTime = DateTime.Now.Add(LastToLeaveSnoozePeriod);
        }

        private void ShowNotification(string title, string message, params ReminderViewModel.Button[] buttons)
        {
            ShowNotification(title, message, null, buttons);
        }

        private void ShowNotification(string title, string message, Action onClose, params ReminderViewModel.Button[] buttons)
        {
            log.Info($"Displayed '{title}' notification.");

            var model = IoC.Get<ReminderViewModel>();
            model.Title = title;
            model.Message = message;
            model.Buttons = buttons;
            notificationManager.Show(model, expirationTime: TimeSpan.FromMinutes(15),
                onClose: () =>
                {
                    onClose?.Invoke();
                    log.Info($"User dismissed '{title}' notification.");
                });
        }
    }
}
