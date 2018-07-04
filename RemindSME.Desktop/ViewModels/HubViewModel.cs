using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using RemindSME.Desktop.Events;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.Views;

namespace RemindSME.Desktop.ViewModels
{
    public class HubViewModel : Screen, IHandle<NextHibernationTimeUpdatedEvent>
    {
        private readonly IActionTracker actionTracker;
        private readonly IHibernationManager hibernationManager;
        private readonly IReminderManager reminderManager;
        private readonly IAppWindowManager appWindowManager;

        public HubViewModel(
            IActionTracker actionTracker,
            IEventAggregator eventAggregator,
            IHibernationManager hibernationManager,
            IReminderManager reminderManager,
            IAppWindowManager appWindowManager)
        {
            this.actionTracker = actionTracker;
            this.hibernationManager = hibernationManager;
            this.reminderManager = reminderManager;
            this.appWindowManager = appWindowManager;

            eventAggregator.Subscribe(this);
        }

        public bool HeatingOptIn
        {
            get => reminderManager.HeatingOptIn;
            set
            {
                if (value == reminderManager.HeatingOptIn)
                {
                    return;
                }

                actionTracker.Log($"User opted {(value ? "in to" : "out of")} heating notifications.");
                reminderManager.HeatingOptIn = value;
                NotifyOfPropertyChange(() => HeatingOptIn);
            }
        }

        public bool HibernationOptIn
        {
            get => hibernationManager.HibernationOptIn;
            set
            {
                if (value == hibernationManager.HibernationOptIn)
                {
                    return;
                }

                actionTracker.Log($"User opted {(value ? "in to" : "out of")} scheduled hibernation.");
                hibernationManager.HibernationOptIn = value;
                NotifyOfPropertyChange(() => HibernationOptIn);
                NotifyOfPropertyChange(() => HibernationOptionIsVisible);
            }
        }

        public bool HibernationOptionIsVisible => hibernationManager.HibernationOptIn;

        public IEnumerable<string> HibernateHours => Enumerable.Range(0, 24).Select(x => x.ToString("D2"));

        public string SelectedHibernateHour
        {
            get => hibernationManager.DefaultHibernationTime.Hours.ToString("D2");
            set
            {
                var defaultHibernationTime = hibernationManager.DefaultHibernationTime;

                var hours = int.Parse(value);
                var minutes = defaultHibernationTime.Minutes;
                var time = new TimeSpan(hours, minutes, 0);
                if (time == defaultHibernationTime)
                {
                    return;
                }

                actionTracker.Log($"User set hibernation time to {time}.");
                hibernationManager.DefaultHibernationTime = time;
                NotifyOfPropertyChange(() => SelectedHibernateHour);
            }
        }

        public IEnumerable<string> HibernateMinutes => Enumerable.Range(0, 4).Select(x => (x * 15).ToString("D2"));

        public string SelectedHibernateMinute
        {
            get => hibernationManager.DefaultHibernationTime.Minutes.ToString("D2");
            set
            {
                var defaultHibernationTime = hibernationManager.DefaultHibernationTime;

                var hours = defaultHibernationTime.Hours;
                var minutes = int.Parse(value);
                var time = new TimeSpan(hours, minutes, 0);
                if (time == defaultHibernationTime)
                {
                    return;
                }

                actionTracker.Log($"User set hibernation time to {time}.");
                hibernationManager.DefaultHibernationTime = time;
                NotifyOfPropertyChange(() => SelectedHibernateMinute);
            }
        }

        public string NextHibernationTime
        {
            get
            {
                var nextHibernationTime = hibernationManager.NextHibernationTime;
                var time = nextHibernationTime.ToShortTimeString();
                var date = nextHibernationTime.Date;

                if (date == DateTime.Today)
                {
                   return $"Today at {time}";
                }
                if (date == DateTime.Today.AddDays(1))
                {
                    return $"Tomorrow at {time}";
                }
                return $"{date:d} at {time}";
            }
        } 

        public string AppVersion => $"Current version: {AppInfo.Version} ({ConfigurationManager.AppSettings["Configuration"]})";
        public string CompanyName => Settings.Default.CompanyName;

        public void Handle(NextHibernationTimeUpdatedEvent message)
        {
            NotifyOfPropertyChange(() => NextHibernationTime);
        }

        public void NavigateTo(string url)
        {
            Process.Start(new ProcessStartInfo(url));
        }

        public void OnClose()
        {
            actionTracker.Log("User closed Hub window.");
            ShowExplanationText = false;
        }

        public void OpenFaqWindow()
        {
            appWindowManager.OpenOrActivateWindow<FaqView, FaqViewModel>();
        }

        public bool ShowExplanationText
        {
            get => Settings.Default.DisplaySettingExplanations;
            set
            {
                Settings.Default.DisplaySettingExplanations = value;
                Settings.Default.Save();
            }
        }

        public void CloseWindow()
        {
            actionTracker.Log("User dismissed Hub window by clicking OK button.");
            TryClose();
        }
    }
}
