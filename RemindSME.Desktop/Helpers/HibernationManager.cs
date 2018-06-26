using System;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.ViewModels;

namespace RemindSME.Desktop
{
    public class HibernationManager
    {
        public bool HibernationPromptHasBeenShown = false;

        public void UpdateNextHiberationTime()
        {
            Settings.Default.NextHibernationTime = Settings.Default.DefaultHibernationTime.Subtract(DateTime.Now.TimeOfDay) > TimeSpan.FromMinutes(15)
                ? DateTime.Today.Add(Settings.Default.DefaultHibernationTime)
                : DateTime.Today.AddDays(1).Add(Settings.Default.DefaultHibernationTime);
            Settings.Default.Save();
        }

        public void Snooze(TimeSpan timespan)
        {
            Settings.Default.NextHibernationTime = Settings.Default.NextHibernationTime.Add(timespan);
        }

        public void NotTonight()
        {
            SetNextHiberateToTomorrow();
        }

        public void Hibernate()
        {
            SetNextHiberateToTomorrow();

            //            System.Windows.Forms.Application.SetSuspendState(PowerState.Hibernate, false, false);

            MessageBox.Show("Hibernate", "RemindS ME",
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
        }

        public void SetNextHiberateToTomorrow()
        {
            Settings.Default.NextHibernationTime = DateTime.Today.AddDays(1).Add(Settings.Default.DefaultHibernationTime);
            Settings.Default.Save();
        }

        public void HandleHibernationOnChange(TimeSpan newDefaultHibernationTime)
        {
            // If the new default time is set to at least 15 minutes in the future, then set it for today. Otherwise set it for tomorrow.
            Settings.Default.NextHibernationTime = newDefaultHibernationTime.Subtract(TimeSpan.FromMinutes(15)) > DateTime.Now.TimeOfDay
                ? DateTime.Today.Add(newDefaultHibernationTime)
                : DateTime.Today.AddDays(1).Add(newDefaultHibernationTime);
            Settings.Default.Save();

            this.HibernationPromptHasBeenShown = false;
        }
    }
}
