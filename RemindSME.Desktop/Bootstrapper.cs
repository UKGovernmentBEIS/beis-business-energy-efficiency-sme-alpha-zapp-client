using System.Reflection;
using System.Windows;
using Autofac;
using Caliburn.Micro.Autofac;
using Notifications.Wpf;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.ViewModels;

namespace RemindSME.Desktop
{
    public class Bootstrapper : AutofacBootstrapper<MainViewModel>
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<NotificationManager>().As<INotificationManager>().SingleInstance();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            Container.Resolve<IHibernationManager>().UpdateNextHiberationTime();
            DisplayRootViewFor<MainViewModel>();
        }
    }
}
