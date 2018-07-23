using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Events;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Logging;
using RemindSME.Desktop.Views;
using static RemindSME.Desktop.Logging.TrackedActions;

namespace RemindSME.Desktop.ViewModels
{
    public class HubViewModel : Screen, IHandle<SettingChangedEvent>
    {
        private readonly IActionLog log;
        private readonly IAppWindowManager appWindowManager;
        private readonly ISettings settings;

        public HubViewModel(
            IActionLog log,
            IAppWindowManager appWindowManager,
            IEventAggregator eventAggregator,
            ISettings settings)
        {
            this.log = log;
            this.appWindowManager = appWindowManager;
            this.settings = settings;

            eventAggregator.Subscribe(this);
        }

        public bool HeatingOptIn
        {
            get => settings.HeatingOptIn;
            set
            {
                if (value == settings.HeatingOptIn)
                {
                    return;
                }

                log.Info($"User opted {(value ? "in to" : "out of")} heating notifications.");
                settings.HeatingOptIn = value;
                NotifyOfPropertyChange(() => HeatingOptIn);
            }
        }

        public bool HibernationOptIn
        {
            get => settings.HibernationOptIn;
            set
            {
                if (value == settings.HibernationOptIn)
                {
                    return;
                }

                var isOptingIn = value;
                var trackedAction = isOptingIn ? OptInToHibernate : OptOutOfHibernate;
                log.Info(trackedAction, $"User opted {(isOptingIn ? "in to" : "out of")} scheduled hibernation.");
                settings.HibernationOptIn = isOptingIn;
                NotifyOfPropertyChange(() => HibernationOptIn);
                NotifyOfPropertyChange(() => HibernationOptionIsVisible);
            }
        }

        public bool HibernationOptionIsVisible => settings.HibernationOptIn;

        public IEnumerable<string> HibernateHours => Enumerable.Range(0, 24).Select(x => x.ToString("D2"));

        public string SelectedHibernateHour
        {
            get => settings.DefaultHibernationTime.Hours.ToString("D2");
            set
            {
                var defaultHibernationTime = settings.DefaultHibernationTime;

                var hours = int.Parse(value);
                var minutes = defaultHibernationTime.Minutes;
                var time = new TimeSpan(hours, minutes, 0);
                if (time == defaultHibernationTime)
                {
                    return;
                }

                log.Info($"User set hibernation time to {time}.");
                settings.DefaultHibernationTime = time;
                NotifyOfPropertyChange(() => SelectedHibernateHour);
            }
        }

        public IEnumerable<string> HibernateMinutes => Enumerable.Range(0, 4).Select(x => (x * 15).ToString("D2"));

        public string SelectedHibernateMinute
        {
            get => settings.DefaultHibernationTime.Minutes.ToString("D2");
            set
            {
                var defaultHibernationTime = settings.DefaultHibernationTime;

                var hours = defaultHibernationTime.Hours;
                var minutes = int.Parse(value);
                var time = new TimeSpan(hours, minutes, 0);
                if (time == defaultHibernationTime)
                {
                    return;
                }

                log.Info($"User set hibernation time to {time}.");
                settings.DefaultHibernationTime = time;
                NotifyOfPropertyChange(() => SelectedHibernateMinute);
            }
        }

        public string NextHibernationTime
        {
            get
            {
                var nextHibernationTime = settings.NextHibernationTime;
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
        public string CompanyName => settings.CompanyName;

        public bool ShowExplanationText
        {
            get => settings.DisplaySettingExplanations;
            set => settings.DisplaySettingExplanations = value;
        }

        public void Handle(SettingChangedEvent message)
        {
            if (message.SettingName == nameof(ISettings.NextHibernationTime))
            {
                NotifyOfPropertyChange(() => NextHibernationTime);
            }
        }

        public void NavigateTo(string url)
        {
            Process.Start(new ProcessStartInfo(url));
        }

        public void OnClose()
        {
            log.Info("User closed Hub window.");
            ShowExplanationText = false;
        }

        public void OpenFaqWindow()
        {
            appWindowManager.OpenOrActivateWindow<FaqView, FaqViewModel>();
        }

        public void CloseWindow()
        {
            log.Info("User dismissed Hub window by clicking OK button.");
            TryClose();
        }
    }
}
