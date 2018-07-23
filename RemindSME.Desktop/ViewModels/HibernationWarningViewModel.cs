using System;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Logging;
using RemindSME.Desktop.Services;
using static RemindSME.Desktop.Logging.TrackedActions;

namespace RemindSME.Desktop.ViewModels
{
    public class HibernationWarningViewModel : Notification
    {
        private readonly IHibernationService hibernationService;
        private readonly IActionLog log;
        private readonly ISettings settings;

        public HibernationWarningViewModel(
            IActionLog log,
            IHibernationService hibernationService,
            ISettings settings,
            DispatcherTimer timer,
            IEventAggregator eventAggregator) : base(eventAggregator)
        {
            this.log = log;
            this.hibernationService = hibernationService;
            this.settings = settings;

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick_UpdateWindow;
            timer.Start();
        }

        public string TimeToNextHibernation => FormatTimeSpan(settings.NextHibernationTime - DateTime.Now);

        public void Snooze()
        {
            CloseWindow();
            log.Info("User clicked 'Snooze' on hibernation warning modal.");
            hibernationService.Snooze(TimeSpan.FromHours(1));
        }

        public void NotTonight()
        {
            CloseWindow();
            log.Info(ClickedNotTonight, "User clicked 'Not tonight' on hibernation warning modal.");
            hibernationService.NotTonight();
        }

        private void CloseWindow()
        {
            (GetView() as Window)?.Close();
        }

        private void Timer_Tick_UpdateWindow(object sender, EventArgs e)
        {
            var timeToHibernate = settings.NextHibernationTime - DateTime.Now;
            if (timeToHibernate < TimeSpan.FromSeconds(1))
            {
                CloseWindow();
            }
            else
            {
                NotifyOfPropertyChange(() => TimeToNextHibernation);
            }
        }

        private static string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalMinutes < 1.0)
            {
                return $"{timeSpan.Seconds} seconds";
            }
            if (timeSpan.TotalHours < 1.0)
            {
                return $"{timeSpan.Minutes} minutes";
            }

            var minutesText = timeSpan.Minutes > 1 ? "minutes" : "minute";
            var hoursText = (int)timeSpan.TotalHours > 1 ? "hours" : "hour";
            return $"{(int)timeSpan.TotalHours} {hoursText}, {timeSpan.Minutes} {minutesText}";
        }
    }
}
