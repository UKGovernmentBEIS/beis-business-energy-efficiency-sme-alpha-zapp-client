using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Windows;
using Autofac;
using Bogus;
using Caliburn.Micro.Autofac;
using Custom.Windows;
using Notifications.Wpf;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.ViewModels;
using RemindSME.Desktop.Views;
using Squirrel;

namespace RemindSME.Desktop
{
    public class Bootstrapper : AutofacBootstrapper<MainViewModel>
    {
        private static readonly string UpdateUrl = ConfigurationManager.AppSettings["UpdateUrl"];

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void PrepareApplication()
        {
            var instanceAwareApplication = (InstanceAwareApplication)Application;
            instanceAwareApplication.StartupNextInstance += InstanceAwareApplication_StartupNextInstance;
            base.PrepareApplication();
        }

        protected override void ConfigureBootstrapper()
        {
            if (string.IsNullOrEmpty(Settings.Default.Pseudonym))
            {
                Settings.Default.Pseudonym = new Faker().Name.FindName(withPrefix: false, withSuffix: false);
                Settings.Default.Save();
            }

            base.ConfigureBootstrapper();
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<NotificationManager>().As<INotificationManager>().SingleInstance();

            if (!string.IsNullOrEmpty(UpdateUrl))
            {
                builder.RegisterInstance(new UpdateManager(UpdateUrl)).As<IUpdateManager>().SingleInstance();
                builder.RegisterType<AppUpdateManager>().As<IAppUpdateManager>().SingleInstance();
            }
            else
            {
                builder.RegisterType<DummyAppUpdateManager>().As<IAppUpdateManager>().SingleInstance();
            }

            base.ConfigureContainer(builder);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            var instanceAwareApplication = (InstanceAwareApplication)Application;
            if (!instanceAwareApplication.IsFirstInstance.GetValueOrDefault() && !IsRelaunchAfterUpdate(Environment.GetCommandLineArgs()))
            {
                Environment.Exit(0);
            }

            Container.Resolve<IHibernationManager>().UpdateNextHiberationTime();
            DisplayRootViewFor<MainViewModel>();
        }

        private void InstanceAwareApplication_StartupNextInstance(object sender, StartupNextInstanceEventArgs e)
        {
            if (!IsRelaunchAfterUpdate(e.Args))
            {
                Container.Resolve<IAppWindowManager>().OpenOrActivateWindow<HubView, HubViewModel>();
            }
        }

        private static bool IsRelaunchAfterUpdate(string[] commandLineArgs)
        {
            return commandLineArgs.Any(arg => arg.Contains("--squirrel"));
        }
    }
}
