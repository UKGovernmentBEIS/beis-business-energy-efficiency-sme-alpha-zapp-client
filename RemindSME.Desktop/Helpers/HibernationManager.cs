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
        TimeSpan DefaultHibernationTime { get; }
        DateTime NextHibernationTime { get; }

        void Hibernate();
        void Snooze();
        void NotTonight();
        void SetDefaultHibernationTime(TimeSpan time);
        void UpdateNextHiberationTime();
    }

    public class HibernationManager : IHibernationManager
    {
        private readonly IEventAggregator eventAggregator;

        public HibernationManager(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public TimeSpan DefaultHibernationTime => Settings.Default.DefaultHibernationTime;
        public DateTime NextHibernationTime => Settings.Default.NextHibernationTime;

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
            SetNextHibernationTime(Settings.Default.NextHibernationTime.Add(SnoozeTime));
        }

        public void NotTonight()
        {
            SetNextHiberateToTomorrow();
        }

        public void UpdateNextHiberationTime()
        {
            var defaultHibernationTime = Settings.Default.DefaultHibernationTime;
            var defaultHibernationTimeIsWithinPromptPeriod = defaultHibernationTime.Subtract(DateTime.Now.TimeOfDay) <= HibernationPromptPeriod;
            var defaultHibernationTimeToday = DateTime.Today.Add(defaultHibernationTime);

            // If within 15 minutes of next potential hibernation, push until tomorrow.
            var nextHibernationTime = defaultHibernationTimeIsWithinPromptPeriod ? defaultHibernationTimeToday.AddDays(1) : defaultHibernationTimeToday;
            SetNextHibernationTime(nextHibernationTime);
        }

        public void SetDefaultHibernationTime(TimeSpan time)
        {
            Settings.Default.DefaultHibernationTime = time;
            UpdateNextHiberationTime(); // Includes settings save.
        }

        private void SetNextHiberateToTomorrow()
        {
            var tomorrow = DateTime.Today.AddDays(1);
            SetNextHibernationTime(tomorrow.Add(Settings.Default.DefaultHibernationTime));
        }

        private void SetNextHibernationTime(DateTime time)
        {
            Settings.Default.NextHibernationTime = time;
            Settings.Default.Save();

            eventAggregator.PublishOnUIThread(new NextHibernationTimeUpdatedEvent());
        }
    }
}
