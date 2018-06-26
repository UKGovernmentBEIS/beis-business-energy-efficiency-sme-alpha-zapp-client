using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using RemindSME.Desktop.Properties;

namespace RemindSME.Desktop.ViewModels
{
    public class HubViewModel : PropertyChangedBase
    {
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

        public bool LightingOptIn
        {
            get => Settings.Default.LightingOptIn;
            set
            {
                if (value == Settings.Default.LightingOptIn)
                {
                    return;
                }

                Settings.Default.LightingOptIn = value;
                Settings.Default.Save();
                NotifyOfPropertyChange(() => LightingOptIn);
            }
        }

        public IEnumerable<string> HibernateHours => Enumerable.Range(0, 24).Reverse().Select(x => x.ToString("D2"));

        public string SelectedHibernateHour
        {
            get => Settings.Default.DefaultHibernationTime.Hours.ToString("D2");
            set
            {
                var hours = int.Parse(value);
                var minutes = Settings.Default.DefaultHibernationTime.Minutes;
                var timespan = new TimeSpan(hours, minutes, 0);
                if (timespan == Settings.Default.DefaultHibernationTime)
                {
                    return;
                }

                Settings.Default.DefaultHibernationTime = timespan;
                Settings.Default.NextHibernationTime = DateTime.Today.Add(Settings.Default.DefaultHibernationTime);
                Settings.Default.Save();
                NotifyOfPropertyChange(() => HibernateHours);
            }
        }

        public IEnumerable<string> HibernateMinutes => Enumerable.Range(0, 4).Select(x => (x * 15).ToString("D2"));

        public string SelectedHibernateMinute
        {
            get => Settings.Default.DefaultHibernationTime.Minutes.ToString("D2");
            set
            {
                var hours = Settings.Default.DefaultHibernationTime.Hours;
                var minutes = int.Parse(value);
                var timespan = new TimeSpan(hours, minutes, 0);
                if (timespan == Settings.Default.DefaultHibernationTime)
                {
                    return;
                }

                Settings.Default.DefaultHibernationTime = timespan;
                Settings.Default.NextHibernationTime = DateTime.Today.Add(Settings.Default.DefaultHibernationTime);
                Settings.Default.Save();
                NotifyOfPropertyChange(() => HibernateMinutes);
            }
        }

        public void NavigateTo(string url)
        {
            Process.Start(new ProcessStartInfo(url));
        }
    }
}
