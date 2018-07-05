using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using Caliburn.Micro;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Events;
using RemindSME.Desktop.Properties;
using static RemindSME.Desktop.Helpers.HibernationSettings;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;

namespace RemindSME.Desktop.Helpers
{
    public interface IHibernationManager
    {
        TimeSpan DefaultHibernationTime { get; set; }
        DateTime NextHibernationTime { get; }
        bool HibernationOptIn { get; set; }

        void Hibernate();
        void Snooze();
        void NotTonight();
        void UpdateNextHiberationTime();
    }

    public class HibernationManager : IHibernationManager
    {
        private readonly IActionTracker actionTracker;
        private readonly IEventAggregator eventAggregator;
        private readonly ISettings settings;

        public HibernationManager(IActionTracker actionTracker, IEventAggregator eventAggregator, ISettings settings)
        {
            this.actionTracker = actionTracker;
            this.eventAggregator = eventAggregator;
            this.settings = settings;
        }

        public TimeSpan DefaultHibernationTime
        {
            get => settings.DefaultHibernationTime;
            set
            {
                settings.DefaultHibernationTime = value;
                UpdateNextHiberationTime();
            }
        }

        public DateTime NextHibernationTime
        {
            get => settings.NextHibernationTime;
            private set
            {
                settings.NextHibernationTime = value;
                eventAggregator.PublishOnUIThread(new NextHibernationTimeUpdatedEvent());
            }
        }

        public bool HibernationOptIn
        {
            get => settings.HibernationOptIn;
            set => settings.HibernationOptIn = value;
        }

        public void Hibernate()
        {
            SetNextHiberateToTomorrow();
            actionTracker.Log("Machine was hibernated automatically.");

            if (Debugger.IsAttached)
            {
                MessageBox.Show("Hibernate", "Zapp",
                    MessageBoxButton.OK,
                    MessageBoxImage.None,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                Application.SetSuspendState(PowerState.Hibernate, false, false);
            }
        }

        public void Snooze()
        {
            NextHibernationTime = NextHibernationTime.Add(SnoozeTime);
        }

        public void NotTonight()
        {
            SetNextHiberateToTomorrow();
        }

        public void UpdateNextHiberationTime()
        {
            // If the hibernation time today has already passed or is within the prompt period, push until tomorrow.
            var pushToTomorrow = DateTime.Now.TimeOfDay >= DefaultHibernationTime.Subtract(HibernationPromptPeriod);

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            NextHibernationTime = pushToTomorrow ? tomorrow.Add(DefaultHibernationTime) : today.Add(DefaultHibernationTime);
        }

        private void SetNextHiberateToTomorrow()
        {
            var tomorrow = DateTime.Today.AddDays(1);
            NextHibernationTime = tomorrow.Add(DefaultHibernationTime);
        }
    }
}
