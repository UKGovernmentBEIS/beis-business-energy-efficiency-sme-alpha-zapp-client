using System.Windows;
using Caliburn.Micro;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.Views;

namespace RemindSME.Desktop.ViewModels
{
    public class WelcomeViewModel : ViewAware
    {
        private readonly ISingletonWindowManager singletonWindowManager;

        public WelcomeViewModel(ISingletonWindowManager singletonWindowManager)
        {
            this.singletonWindowManager = singletonWindowManager;
        }

        private Window Window => GetView() as Window;

        public void OpenHubWindow()
        {
            Window.Close();

            Settings.Default.DisplaySettingExplanations = true;
            Settings.Default.Save();

            singletonWindowManager.OpenOrActivateSingletonWindow<HubView, HubViewModel>();
        }
    }
}
