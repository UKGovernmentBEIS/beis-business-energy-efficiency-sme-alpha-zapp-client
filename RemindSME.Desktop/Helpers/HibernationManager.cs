﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using Caliburn.Micro;
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

        void Hibernate();
        void Snooze();
        void NotTonight();
        void UpdateNextHiberationTime();
    }

    public class HibernationManager : IHibernationManager
    {
        private readonly IActionTracker actionTracker;
        private readonly IEventAggregator eventAggregator;

        public HibernationManager(IActionTracker actionTracker, IEventAggregator eventAggregator)
        {
            this.actionTracker = actionTracker;
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

        public void Hibernate()
        {
            SetNextHiberateToTomorrow();
            actionTracker.Log("Machine was hibernated automatically.");

            if (Debugger.IsAttached)
            {
                MessageBox.Show("Hibernate", "RemindS ME",
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
