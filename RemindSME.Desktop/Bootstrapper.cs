using System;
using System.Windows;
using Autofac;
using Caliburn.Micro.Autofac;
using Notifications.Wpf;
using RemindSME.Desktop.ViewModels;
using RemindSME.Desktop.Properties;

namespace RemindSME.Desktop
{
    public class Bootstrapper : AutofacBootstrapper<TaskbarIconViewModel>
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<NotificationManager>().As<INotificationManager>().SingleInstance();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // If the next hibernation time is today, but in the past, or within the next 15 minutes, then bump it along to the default time tomorrow
            if (Settings.Default.NextHibernationTime < DateTime.Now.AddMinutes(15))
            {
                Settings.Default.NextHibernationTime = DateTime.Today.AddDays(1).Add(Settings.Default.DefaultHibernationTime);
                Settings.Default.Save();
            }

            DisplayRootViewFor<TaskbarIconViewModel>();
        }
    }
}
