using System;
using System.Windows;
using System.Windows.Threading;
using RemindsSME.Desktop.Properties;

//using PowerState = System.Windows.Forms.PowerState;

namespace RemindsSME.Desktop.ViewModels
{
    public class TaskbarIconViewModel
    {
        private static readonly TimeSpan HibernationTime = new TimeSpan(18, 00, 00); // 18:00

        public TaskbarIconViewModel()
        {
            var timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var alreadyHibernatedToday = DateTime.Today <= Settings.Default.LastScheduledHibernate;
            if (alreadyHibernatedToday || DateTime.Now.TimeOfDay < HibernationTime)
            {
                return;
            }
            Settings.Default.LastScheduledHibernate = DateTime.Today;
            Settings.Default.Save();
            Hibernate();
        }


        public void Hibernate()
        {
//            System.Windows.Forms.Application.SetSuspendState(PowerState.Hibernate, false, false);
            MessageBox.Show("Hibernate", "RemindS ME",
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
        }

        public void Quit()
        {
            Application.Current.Shutdown();
        }
    }
}