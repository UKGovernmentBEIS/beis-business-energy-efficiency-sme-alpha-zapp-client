using System.Configuration;
using System.Reflection;
using System.Windows;
using Autofac;
using Caliburn.Micro.Autofac;
using Notifications.Wpf;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.ViewModels;
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
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            Container.Resolve<IHibernationManager>().UpdateNextHiberationTime();
            DisplayRootViewFor<MainViewModel>();
        }
    }
}
