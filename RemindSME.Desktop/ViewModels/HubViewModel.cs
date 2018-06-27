using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using RemindSME.Desktop.Events;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Properties;

namespace RemindSME.Desktop.ViewModels
{
    public class HubViewModel : PropertyChangedBase, IHandle<NextHibernationTimeUpdatedEvent>
    {
        private readonly IHibernationManager hibernationManager;

        public HubViewModel(IHibernationManager hibernationManager, IEventAggregator eventAggregator)
        {
            this.hibernationManager = hibernationManager;
            eventAggregator.Subscribe(this);
        }

        public bool HeatingOptIn
        {
            get => Settings.Default.HeatingOptIn;
            set
            {
                if (value == Settings.Default.HeatingOptIn)
                {
                    return;
                }

                Settings.Default.HeatingOptIn = value;
                Settings.Default.Save();
                NotifyOfPropertyChange(() => HeatingOptIn);
            }
        }

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

        public string NextHibernationTime => $"Next scheduled hibernation: {hibernationManager.NextHibernationTime:f}";

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
