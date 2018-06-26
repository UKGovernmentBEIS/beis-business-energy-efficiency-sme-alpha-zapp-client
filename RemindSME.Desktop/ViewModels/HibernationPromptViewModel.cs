using System;
using Caliburn.Micro;
using System.Windows;
using RemindSME.Desktop.Properties;

namespace RemindSME.Desktop.ViewModels
{
    public class HibernationPromptViewModel : PropertyChangedBase
    {
        private readonly HibernationManager hibernationManager;

        public HibernationPromptViewModel()
        {
            this.hibernationManager = new HibernationManager();
        }

        public void DoItNow()
        {
            hibernationManager.Hibernate();

            MessageBox.Show("Do it now", "RemindS ME",
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
        }

        public void Snooze()
        {
            hibernationManager.Snooze(TimeSpan.FromHours(1));

            MessageBox.Show("Hibernation snoozed by 1 hour", "RemindS ME",
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
        }

        public void NotTonight()
        {
            hibernationManager.NotTonight();

            MessageBox.Show("Your computer will not hibernate today", "RemindS ME",
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
        }
    }
}
