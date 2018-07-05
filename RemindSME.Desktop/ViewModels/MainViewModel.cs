using System.Windows;
using Caliburn.Micro;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Views;
using Squirrel;

namespace RemindSME.Desktop.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        private readonly IAppWindowManager appWindowManager;
        private readonly ILog log;

        public MainViewModel(
            ILog log,
            IAppWindowManager appWindowManager,
            IEventAggregator eventAggregator)
        {
            this.log = log;
            this.appWindowManager = appWindowManager;

            eventAggregator.Subscribe(this);

            SquirrelAwareApp.HandleEvents(onFirstRun: OpenWelcomeWindow);
        }

        public void OpenWelcomeWindow()
        {
            appWindowManager.OpenOrActivateWindow<WelcomeView, WelcomeViewModel>();
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
