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
            DisplayRootViewFor<TaskbarIconViewModel>();
        }
    }
}
