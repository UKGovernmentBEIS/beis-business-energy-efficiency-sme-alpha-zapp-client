using System.Windows;
using Caliburn.Micro.Autofac;
using RemindSME.Desktop.ViewModels;

namespace RemindSME.Desktop
{
    public class Bootstrapper : AutofacBootstrapper<TaskbarIconViewModel>
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<TaskbarIconViewModel>();
        }
    }
}