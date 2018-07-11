using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using Bogus;
using Caliburn.Micro;
using Caliburn.Micro.Autofac;
using Custom.Windows;
using Notifications.Wpf;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Helpers.Listeners;
using RemindSME.Desktop.Logging;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.Services;
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

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces().SingleInstance();

            builder.RegisterAssemblyTypes(assembly).Where(t => t.IsAssignableTo<IService>()).AsSelf().SingleInstance();
            builder.RegisterAssemblyTypes(assembly).Where(t => t.IsAssignableTo<SocketListener>()).AsSelf().SingleInstance();

            builder.RegisterType<NotificationManager>().AsSelf().SingleInstance();
            builder.RegisterInstance(Settings.Default).As<ISettings>().SingleInstance();

            builder.RegisterType<DispatcherTimer>().AsSelf().InstancePerDependency();
            builder.RegisterType<Logger>().As<ILog>().InstancePerDependency();

            if (!string.IsNullOrEmpty(UpdateUrl))
            {
                builder.RegisterInstance(new UpdateManager(UpdateUrl)).As<IUpdateManager>().SingleInstance();
            }
            else
            {
                builder.RegisterType<DummyUpdateManager>().As<IUpdateManager>().SingleInstance();
            }

            base.ConfigureContainer(builder);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            InitializeSettings();
            InitializeServices();

            var instanceAwareApplication = (InstanceAwareApplication)Application;
            if (!instanceAwareApplication.IsFirstInstance.GetValueOrDefault() && !IsRelaunchAfterUpdate(Environment.GetCommandLineArgs()))
            {
                Application.Current.Shutdown();
            }

            DisplayRootViewFor<MainViewModel>();

            base.OnStartup(sender, e);
        }

        private void InitializeSettings()
        {
            var settings = Container.Resolve<ISettings>();
            if (string.IsNullOrEmpty(settings.Pseudonym))
            {
                settings.Pseudonym = new Faker().Name.FindName(withPrefix: false, withSuffix: false);
            }
        }

        private void InitializeServices()
        {
            var serviceTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsInterface && type.IsAssignableTo<IService>());
            foreach (var serviceType in serviceTypes)
            {
                var service = (IService)Container.Resolve(serviceType);
                service.Initialize();
            }
        }

        private void InstanceAwareApplication_StartupNextInstance(object sender, StartupNextInstanceEventArgs e)
        {
            if (!IsRelaunchAfterUpdate(e.Args))
            {
                Container.Resolve<IAppWindowManager>().OpenOrActivateWindow<HubView, HubViewModel>();
            }
        }

        private static bool IsRelaunchAfterUpdate(IEnumerable<string> commandLineArgs)
        {
            return commandLineArgs.Any(arg => arg.Contains("--squirrel"));
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
                e.Handled = true;
            }
            else
            {
                Container.Resolve<ILog>().Error(e.Exception);
                e.Handled = true;
            }

            base.OnUnhandledException(sender, e);
        }
    }
}
