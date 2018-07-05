using System.Windows;
using Caliburn.Micro;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.Views;

namespace RemindSME.Desktop.ViewModels
{
    public class WelcomeViewModel : ViewAware
    {
        private readonly IAppWindowManager appWindowManager;

        public WelcomeViewModel(IAppWindowManager appWindowManager)
        {
            this.appWindowManager = appWindowManager;
        }

        public void OpenHubWindow()
        {
            (GetView() as Window)?.Close();

            Settings.Default.DisplaySettingExplanations = true;
            Settings.Default.Save();

            appWindowManager.OpenOrActivateWindow<HubView, HubViewModel>();
        }
    }
}
