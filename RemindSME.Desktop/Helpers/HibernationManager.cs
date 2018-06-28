using System;
using System.Windows;
using Caliburn.Micro;
using RemindSME.Desktop.Events;
using RemindSME.Desktop.Properties;
using static RemindSME.Desktop.Helpers.HibernationSettings;
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
        private readonly IEventAggregator eventAggregator;

        public HibernationManager(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public TimeSpan DefaultHibernationTime
        {
            get => Settings.Default.DefaultHibernationTime;
            set
            {
                Settings.Default.DefaultHibernationTime = value;
                UpdateNextHiberationTime();
            }
        }

        public DateTime NextHibernationTime
        {
            get => Settings.Default.NextHibernationTime;
            private set
            {
                Settings.Default.NextHibernationTime = value;
                Settings.Default.Save();

                eventAggregator.PublishOnUIThread(new NextHibernationTimeUpdatedEvent());
            }
        }
 
        public bool HibernationOptIn
        {
            get => Settings.Default.HibernationOptIn;
            set
            {
                Settings.Default.HibernationOptIn = value;
                Settings.Default.Save();
            }
        }

        public void Hibernate()
        {
            SetNextHiberateToTomorrow();

            // System.Windows.Forms.Application.SetSuspendState(PowerState.Hibernate, false, false);

            MessageBox.Show("Hibernate", "RemindS ME",
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
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
            var defaultHibernationTimeToday = DateTime.Today.Add(DefaultHibernationTime);
            var defaultHibernationTimeIsWithinPromptPeriod = defaultHibernationTimeToday.Subtract(DateTime.Now) <= HibernationPromptPeriod;

            // If within 15 minutes of next potential hibernation, push until tomorrow.
            NextHibernationTime = defaultHibernationTimeIsWithinPromptPeriod ? defaultHibernationTimeToday.AddDays(1) : defaultHibernationTimeToday;
        }

        private void SetNextHiberateToTomorrow()
        {
            var tomorrow = DateTime.Today.AddDays(1);
            NextHibernationTime = tomorrow.Add(DefaultHibernationTime);
        }
    }
}
