using System.Windows;
using Caliburn.Micro;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Views;

namespace RemindSME.Desktop.ViewModels
{
    public class WelcomeViewModel : ViewAware
    {
        private readonly IAppWindowManager appWindowManager;
        private readonly ISettings settings;

        public WelcomeViewModel(IAppWindowManager appWindowManager, ISettings settings)
        {
            this.appWindowManager = appWindowManager;
            this.settings = settings;
        }

        public void OpenHubWindow()
        {
            (GetView() as Window)?.Close();

            settings.DisplaySettingExplanations = true;
            appWindowManager.OpenOrActivateWindow<HubView, HubViewModel>();
        }
    }
}
