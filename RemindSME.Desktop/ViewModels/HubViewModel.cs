using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using RemindSME.Desktop.Events;
using RemindSME.Desktop.Helpers;

namespace RemindSME.Desktop.ViewModels
{
    public class HubViewModel : PropertyChangedBase, IHandle<NextHibernationTimeUpdatedEvent>
    {
        private readonly IActionTracker actionTracker;
        private readonly IHibernationManager hibernationManager;
        private readonly IReminderManager reminderManager;

        public HubViewModel(
            IActionTracker actionTracker,
            IEventAggregator eventAggregator,
            IHibernationManager hibernationManager,
            IReminderManager reminderManager)
        {
            this.actionTracker = actionTracker;
            this.hibernationManager = hibernationManager;
            this.reminderManager = reminderManager;

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

        public IEnumerable<string> HibernateHours => Enumerable.Range(0, 24).Reverse().Select(x => x.ToString("D2"));

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

        public string Configuration => ConfigurationManager.AppSettings["Configuration"];

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
        }
    }
}
