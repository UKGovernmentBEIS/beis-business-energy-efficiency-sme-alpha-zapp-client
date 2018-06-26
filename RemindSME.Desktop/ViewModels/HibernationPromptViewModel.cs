using System;
using Caliburn.Micro;
using System.Windows;
using RemindSME.Desktop.Properties;

namespace RemindSME.Desktop.ViewModels
{
    public class HibernationPromptViewModel : PropertyChangedBase
    {
        public void DoItNow()
        {
            
            MessageBox.Show("Do it now", "RemindS ME",
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
        }

        public void Snooze()
        {
            HibernationHelper.Snooze(TimeSpan.FromHours(1));

            MessageBox.Show("Hibernation snoozed by 1 hour", "RemindS ME",
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
        }

        public void NotTonight()
        {
            MessageBox.Show("Not Tonight", "RemindS ME",
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
        }

    }
}

