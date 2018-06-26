using Caliburn.Micro;
using System.Windows;

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
            MessageBox.Show("Snooze", "RemindS ME",
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