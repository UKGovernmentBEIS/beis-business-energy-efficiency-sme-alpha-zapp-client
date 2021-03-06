﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Caliburn.Micro;
using Notifications.Wpf;
using Zapp.Desktop.Configuration;
using Zapp.Desktop.Events;
using Zapp.Desktop.Helpers;
using Zapp.Desktop.Logging;
using Zapp.Desktop.ViewModels;
using Zapp.Desktop.Views;
using static Zapp.Desktop.Logging.TrackedActions;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;

namespace Zapp.Desktop.Services
{
    public interface IHibernationService : IService
    {
        void Snooze(TimeSpan snoozeTime);
        void NotTonight();
    }

    public class HibernationService : IHibernationService, IHandle<SettingChangedEvent>
    {
        public static readonly TimeSpan HibernationPromptPeriod = TimeSpan.FromMinutes(15);
        public static readonly TimeSpan HibernationWarningPeriod = TimeSpan.FromSeconds(30);

        private readonly IActionLog log;
        private readonly IAppWindowManager appWindowManager;
        private readonly INotificationManager notificationManager;
        private readonly ISettings settings;
        private readonly DispatcherTimer timer;

        private bool hibernationPromptHasBeenShown;
        private bool hibernationWarningHasBeenShown;

        public HibernationService(
            IActionLog log,
            IAppWindowManager appWindowManager,
            IEventAggregator eventAggregator,
            INotificationManager notificationManager,
            ISettings settings,
            DispatcherTimer timer)
        {
            this.log = log;
            this.appWindowManager = appWindowManager;
            this.notificationManager = notificationManager;
            this.settings = settings;
            this.timer = timer;

            eventAggregator.Subscribe(this);
        }

        public void Initialize()
        {
            UpdateNextHiberationTime();

            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void Handle(SettingChangedEvent e)
        {
            switch (e.SettingName)
            {
                case nameof(ISettings.HibernationOptIn):
                case nameof(ISettings.DefaultHibernationTime):
                    UpdateNextHiberationTime();
                    break;
                case nameof(ISettings.NextHibernationTime):
                    hibernationPromptHasBeenShown = false;
                    hibernationWarningHasBeenShown = false;
                    break;
            }
        }

        public void Snooze(TimeSpan snoozeTime)
        {
            settings.NextHibernationTime = settings.NextHibernationTime.Add(snoozeTime);
        }

        public void NotTonight()
        {
            SetNextHiberateToTomorrow();
        }

        private void Hibernate()
        {
            SetNextHiberateToTomorrow();
            log.Info(ZappHibernation, "Machine was hibernated automatically.");

            if (Debugger.IsAttached)
            {
                MessageBox.Show("Machine would hibernate.", "Zapp", MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                Application.SetSuspendState(PowerState.Hibernate, false, false);
            }
        }

        private void UpdateNextHiberationTime()
        {
            var defaultHibernationTime = settings.DefaultHibernationTime;

            // If the hibernation time today has already passed or is within the prompt period, push until tomorrow.
            var pushToTomorrow = DateTime.Now.TimeOfDay >= defaultHibernationTime.Subtract(HibernationPromptPeriod);

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            settings.NextHibernationTime = pushToTomorrow ? tomorrow.Add(defaultHibernationTime) : today.Add(defaultHibernationTime);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!settings.HibernationOptIn)
            {
                return;
            }

            var nextHibernationTime = settings.NextHibernationTime;

            // Next hibernation time is yesterday or earlier, so should be updated.
            if (nextHibernationTime.Date < DateTime.Today)
            {
                UpdateNextHiberationTime();
                return; // Avoid race conditions by not acting again until next tick.
            }

            var timeUntilHibernation = nextHibernationTime.Subtract(DateTime.Now);
            if (timeUntilHibernation <= TimeSpan.Zero)
            {
                Hibernate();
            }
            else if (timeUntilHibernation <= HibernationWarningPeriod)
            {
                if (!hibernationWarningHasBeenShown)
                {
                    ShowHibernationWarning();
                }
            }
            else if (timeUntilHibernation <= HibernationPromptPeriod)
            {
                if (!hibernationPromptHasBeenShown)
                {
                    ShowHibernationPrompt();
                }
            }
        }

        private void SetNextHiberateToTomorrow()
        {
            var tomorrow = DateTime.Today.AddDays(1);
            settings.NextHibernationTime = tomorrow.Add(settings.DefaultHibernationTime);
        }

        private void ShowHibernationPrompt()
        {
            log.Info("Showed hibernation prompt.");
            hibernationPromptHasBeenShown = true;
            var model = IoC.Get<HibernationPromptViewModel>();
            notificationManager.Show(model, expirationTime: TimeSpan.FromHours(2));
        }

        private void ShowHibernationWarning()
        {
            log.Info("Showed hibernation warning.");
            hibernationWarningHasBeenShown = true;
            appWindowManager.OpenOrActivateDialog<HibernationWarningView, HibernationWarningViewModel>();
        }
    }
}
