using System.Windows;
using Caliburn.Micro;
using RemindSME.Desktop.ViewModels;

namespace RemindSME.Desktop
{
    public class Bootstrapper : BootstrapperBase
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
