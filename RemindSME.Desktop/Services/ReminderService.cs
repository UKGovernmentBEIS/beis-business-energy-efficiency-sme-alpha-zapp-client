﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Caliburn.Micro;
using Notifications.Wpf;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Events;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Models;
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
        private readonly IWeatherApiClient weatherApiClient;
        private readonly ISettings settings;
        private readonly DispatcherTimer timer;

        private bool isShowingFirstLoginReminder;
        private bool isShowingLastToLeaveReminder;

        private int? networkCount;

        public ReminderService(
            ILog log,
            INotificationManager notificationManager,
            IWeatherApiClient weatherApiClient,
            IAppWindowManager appWindowManager,
            IEventAggregator eventAggregator,
            ISettings settings,
            DispatcherTimer timer)
        {
            this.log = log;
            this.notificationManager = notificationManager;
            this.weatherApiClient = weatherApiClient;
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

        public void Handle(NetworkCountChangeEvent e)
        {
            networkCount = e.Count;
        }

        public void Handle(HeatingNotificationEvent e)
        {
            if (ShouldShowHeatingReminder())
            {
                ShowHeatingReminder(e.Title, e.Message);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (appWindowManager.AnyAppWindowIsOpen())
            {
                // Suppress all timed reminders while app windows are open.
                // This avoids immediately showing reminders to new or newly opted-in users.
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

        private bool ShouldShowHeatingReminder()
        {
            return settings.HeatingOptIn;
        }

        private void ShowHeatingReminder(string title, string message)
        {
            ShowReminder(
                title ?? Resources.Reminder_HeatingDefault_Title,
                message ?? Resources.Reminder_HeatingDefault_Message);
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
                   networkCount.HasValue && networkCount.Value <= FirstInThreshold && // Few enough people.
                   settings.MostRecentFirstLoginReminderDismissal.Date != DateTime.Today; // Has not dismissed today.
        }

        private async void ShowFirstLoginReminder()
        {
            isShowingFirstLoginReminder = true;
            var message = await GetWeatherDependentMessage();
            ShowReminder(
                Resources.Reminder_HeatingFirstLogin_Title,
                message,
                () => isShowingFirstLoginReminder = false,
                new ReminderViewModel.Button("Done!", FirstLoginReminder_Done));
        }

        private async Task<string> GetWeatherDependentMessage()
        {
            var forecast = await weatherApiClient.GetWeatherForecastForLocation("London,UK");
            var peakTemperature = GetPeakTemperatureForToday(forecast);
            return peakTemperature != null
                ? $"Looks like it's going to be hot today ({peakTemperature:F0}°C)! Please make sure the air conditioning is set to a sensible temperature for today's weather. Can you open windows instead?"
                : Resources.Reminder_HeatingFirstLogin_Message;
        }

        private double? GetPeakTemperatureForToday(WeatherForecast weatherForecast)
        {
            return weatherForecast?.Forecasts
                .Where(forecast => forecast.Time < DateTime.Today.AddDays(1))
                .Max(forecast => forecast.Measurements.Temperature);
        }

        private void FirstLoginReminder_Done()
        {
            log.Info("User clicked 'Done!' on first login reminder.");
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

        private void ShowReminder(string title, string message, params ReminderViewModel.Button[] buttons)
        {
            ShowReminder(title, message, null, buttons);
        }

        private void ShowReminder(string title, string message, Action onClose, params ReminderViewModel.Button[] buttons)
        {
            log.Info($"Displayed '{title}' reminder.");

            var model = IoC.Get<ReminderViewModel>();
            model.Title = title;
            model.Message = message;
            model.Buttons = buttons;
            notificationManager.Show(model, expirationTime: TimeSpan.FromMinutes(15),
                onClose: () =>
                {
                    if (onClose != null)
                    {
                        onClose.Invoke();
                    }
                    else
                    {
                        log.Info($"User dismissed '{title}' reminder.");
                    }
                });
        }
    }
}
