﻿using System;
using System.Windows.Threading;
using Caliburn.Micro;
using Notifications.Wpf;
using Zapp.Desktop.Configuration;
using Zapp.Desktop.Events;
using Zapp.Desktop.Helpers;
using Zapp.Desktop.Logging;
using Zapp.Desktop.Properties;
using Zapp.Desktop.ViewModels;
using static Zapp.Desktop.Logging.TrackedActions;
using Action = System.Action;

namespace Zapp.Desktop.Services
{
    public class ReminderService : IService, IHandle<NetworkCountChangeEvent>
    {
        private const int LastToLeaveThreshold = 3;

        private static readonly TimeSpan FirstLoginMinimumTime = new TimeSpan(06, 00, 00);
        private static readonly TimeSpan FirstLoginMaximumTime = new TimeSpan(11, 00, 00);
        private static readonly TimeSpan LastToLeaveMinimumTime = new TimeSpan(17, 00, 00);

        private static readonly TimeSpan LastToLeaveSnoozePeriod = TimeSpan.FromMinutes(30);

        private readonly IAppWindowManager appWindowManager;
        private readonly IEventAggregator eventAggregator;
        private readonly IHeatingReminderHelper heatingReminderHelper;
        private readonly IActionLog log;
        private readonly INetworkService networkService;
        private readonly INotificationManager notificationManager;
        private readonly ISettings settings;
        private readonly DispatcherTimer timer;

        private bool isShowingFirstLoginReminder;
        private bool isShowingLastToLeaveReminder;

        private int? networkCount;

        public ReminderService(
            IActionLog log,
            INotificationManager notificationManager,
            IHeatingReminderHelper heatingReminderHelper,
            IAppWindowManager appWindowManager,
            IEventAggregator eventAggregator,
            ISettings settings,
            DispatcherTimer timer,
            INetworkService networkService)
        {
            this.log = log;
            this.notificationManager = notificationManager;
            this.heatingReminderHelper = heatingReminderHelper;
            this.appWindowManager = appWindowManager;
            this.eventAggregator = eventAggregator;
            this.settings = settings;
            this.timer = timer;
            this.networkService = networkService;
        }

        public void Initialize()
        {
            eventAggregator.Subscribe(this);

            timer.Interval = TimeSpan.FromSeconds(30);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void Handle(NetworkCountChangeEvent e)
        {
            networkCount = e.Count;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (appWindowManager.AnyAppWindowIsOpen() || !networkService.IsWorkNetwork)
            {
                // Suppress all timed reminders while app windows are open.
                // This avoids immediately showing reminders to new or newly opted-in users.
                // Also suppresses any notifications if user is not on a work network. 
                return;
            }

            if (ShouldShowLastToLeaveReminder())
            {
                ShowLastToLeaveReminder();
            }

            if (ShouldShowFirstLoginReminder())
            {
                ShowFirstLoginReminder();
            }
        }

        private bool ShouldShowFirstLoginReminder()
        {
            if (isShowingFirstLoginReminder)
            {
                return false;
            }

            var time = DateTime.Now.TimeOfDay;
            return settings.HeatingOptIn && // Opted in.
                   time >= FirstLoginMinimumTime && // Not too early.
                   time <= FirstLoginMaximumTime && // Early enough.
                   settings.MostRecentFirstLoginReminderDismissal.Date != DateTime.Today; // Has not dismissed today.
        }

        private async void ShowFirstLoginReminder()
        {
            isShowingFirstLoginReminder = true;
            var reminder = await heatingReminderHelper.GetWeatherDependentReminder();
            reminder.Buttons = new[]
            {
                new ReminderViewModel.Button("Done!", FirstLoginReminder_Done),
                new ReminderViewModel.Button("Not now", FirstLoginReminder_NotNow)
            };
            ShowReminder(reminder, () => isShowingFirstLoginReminder = false);
        }

        private void FirstLoginReminder_Done()
        {
            log.Info(HeatingFirstLoginDone, "User clicked 'Done!' on first login reminder.");
            settings.MostRecentFirstLoginReminderDismissal = DateTime.Now;
        }

        private void FirstLoginReminder_NotNow()
        {
            log.Info(HeatingFirstLoginNotNow, "User clicked 'Not now' on first login reminder.");
            settings.MostRecentFirstLoginReminderDismissal = DateTime.Now;
        }

        private bool ShouldShowLastToLeaveReminder()
        {
            if (isShowingLastToLeaveReminder)
            {
                return false;
            }

            var now = DateTime.Now;
            return now.TimeOfDay >= LastToLeaveMinimumTime && // Late enough.
                   now >= settings.LastToLeaveReminderSnoozeUntilTime && // After any existing snooze.
                   networkCount.HasValue && networkCount.Value <= LastToLeaveThreshold && // Few enough people.
                   settings.MostRecentLastToLeaveReminderDismissal.Date != DateTime.Today; // Has not dismissed today.
        }

        private void ShowLastToLeaveReminder()
        {
            isShowingLastToLeaveReminder = true;
            ShowReminder(
                Resources.Reminder_LastToLeave_Title,
                Resources.Reminder_LastToLeave_Message,
                () => isShowingLastToLeaveReminder = false,
                new ReminderViewModel.Button("OK", LastToLeaveReminder_OK),
                new ReminderViewModel.Button("Snooze", LastToLeaveReminder_Snooze));
        }

        private void LastToLeaveReminder_OK()
        {
            log.Info("User clicked 'OK' on last to leave reminder.");
            settings.MostRecentLastToLeaveReminderDismissal = DateTime.Now;
        }

        private void LastToLeaveReminder_Snooze()
        {
            log.Info("User clicked 'Snooze' on last to leave reminder.");
            settings.LastToLeaveReminderSnoozeUntilTime = DateTime.Now.Add(LastToLeaveSnoozePeriod);
        }

        private void ShowReminder(string title, string message, Action onClose, params ReminderViewModel.Button[] buttons)
        {
            var model = IoC.Get<ReminderViewModel>();
            model.Title = title;
            model.Message = message;
            model.Buttons = buttons;
            ShowReminder(model, onClose);
        }

        private void ShowReminder(ReminderViewModel model, Action onClose)
        {
            log.Info($"Displayed '{model.Title}' reminder.");
            notificationManager.Show(model, expirationTime: TimeSpan.FromMinutes(15),
                onClose: () =>
                {
                    if (onClose != null)
                    {
                        onClose.Invoke();
                    }
                    else
                    {
                        log.Info($"User dismissed '{model.Title}' reminder.");
                    }
                });
        }
    }
}
