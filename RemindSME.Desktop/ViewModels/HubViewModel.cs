﻿using Caliburn.Micro;
using RemindSME.Desktop.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Navigation;

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

        public IEnumerable<string> HibernateHours
        {
            get => Enumerable.Range(00, 24).Reverse().Select(x => x.ToString("D2"));
        }

        public string SelectedHibernateHour
        {
            get => Settings.Default.HibernateTime.Hours.ToString("D2");
            set
            {
                var timespan = new TimeSpan(Int32.Parse(value), Settings.Default.HibernateTime.Minutes, 0);
                if (timespan == Settings.Default.HibernateTime)
                {
                    return;
                }

                Settings.Default.HibernateTime = timespan;
                Settings.Default.Save();
                NotifyOfPropertyChange(() => HibernateHours);
            }
        }

        public IEnumerable<string> HibernateMinutes
        {
            get => Enumerable.Range(0, 4).Select(x => (x * 15).ToString("D2"));
        }

        public string SelectedHibernateMinute
        {
            get => Settings.Default.HibernateTime.Minutes.ToString("D2");
            set
            {
                var timespan = new TimeSpan(Settings.Default.HibernateTime.Hours, Int32.Parse(value), 0);
                if (timespan == Settings.Default.HibernateTime)
                {
                    return;
                }

                Settings.Default.HibernateTime = timespan;
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
