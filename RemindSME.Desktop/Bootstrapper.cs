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
        private const string UpdateUrl = "https://reminds-me-server.herokuapp.com/Releases";

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsImplementedInterfaces();
            builder.RegisterType<NotificationManager>().As<INotificationManager>().SingleInstance();
            builder.RegisterInstance(new UpdateManager(UpdateUrl)).As<IUpdateManager>().SingleInstance();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            Container.Resolve<IAppUpdateManager>().SetupInstallerEventHandlers();
            Container.Resolve<IHibernationManager>().UpdateNextHiberationTime();
            DisplayRootViewFor<MainViewModel>();
        }
    }
}
