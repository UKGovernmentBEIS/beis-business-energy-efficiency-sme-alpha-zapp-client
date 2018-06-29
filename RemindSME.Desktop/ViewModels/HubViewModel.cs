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
        private readonly IHibernationManager hibernationManager;
        private readonly IReminderManager reminderManager;

        public HubViewModel(
            IEventAggregator eventAggregator,
            IHibernationManager hibernationManager,
            IReminderManager reminderManager)
        {
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

                hibernationManager.DefaultHibernationTime = time;
                NotifyOfPropertyChange(() => SelectedHibernateMinute);
            }
        }

        public string NextHibernationTime
        {
            get
            {
                var message = "";
                if (hibernationManager.NextHibernationTime.Date == DateTime.Today)
                {
                    message = $"Next scheduled hibernation: Today at {hibernationManager.NextHibernationTime:t}";
                }
                else if (hibernationManager.NextHibernationTime.Date == DateTime.Today.AddDays(1))
                {
                    message = $"Next scheduled hibernation: Tomorrow at {hibernationManager.NextHibernationTime:t}";
                }
                else
                {
                    return $"Next scheduled hibernation: {hibernationManager.NextHibernationTime:f}";
                }

                return message;
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
    }
}
