using System.Windows;
using Caliburn.Micro;
using Zapp.Desktop.Configuration;
using Zapp.Desktop.Helpers;
using Zapp.Desktop.Views;

namespace Zapp.Desktop.ViewModels
{
    public class MainViewModel : ViewAware
    {
        private readonly IAppWindowManager appWindowManager;
        private readonly ILog log;
        private readonly ISettings settings;

        public MainViewModel(
            ILog log,
            IAppWindowManager appWindowManager,
            IEventAggregator eventAggregator,
            ISettings settings)
        {
            this.log = log;
            this.appWindowManager = appWindowManager;
            this.settings = settings;

            eventAggregator.Subscribe(this);
        }

        protected override void OnViewLoaded(object view)
        {
            if (string.IsNullOrEmpty(settings.CompanyId))
            {
                appWindowManager.OpenOrActivateWindow<WelcomeView, WelcomeViewModel>();
            }
        }

        public void OpenHubWindow(string actionArea)
        {
            log.Info($"User opened Hub window via taskbar {actionArea}.");
            OpenHubWindow();
        }

        public void OpenHubWindow()
        {
            appWindowManager.OpenOrActivateWindow<HubView, HubViewModel>();
        }

        public void OpenFaqWindow()
        {
            log.Info("User opened FAQ window.");
            appWindowManager.OpenOrActivateWindow<FaqView, FaqViewModel>();
        }

        public void Quit()
        {
            log.Info("User quit the app via taskbar menu click.");
            Application.Current.Shutdown();
        }
    }
}
