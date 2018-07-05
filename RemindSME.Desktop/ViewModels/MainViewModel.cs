using System.Windows;
using Caliburn.Micro;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Views;
using Squirrel;

namespace RemindSME.Desktop.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        private readonly IActionTracker actionTracker;
        private readonly IAppWindowManager appWindowManager;

        public MainViewModel(
            IActionTracker actionTracker,
            IAppWindowManager appWindowManager,
            IEventAggregator eventAggregator)
        {
            this.actionTracker = actionTracker;
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
            actionTracker.Log($"User opened Hub window via taskbar {actionArea}.");
            OpenHubWindow();
        }

        public void OpenHubWindow()
        {
            appWindowManager.OpenOrActivateWindow<HubView, HubViewModel>();
        }

        public void OpenFaqWindow()
        {
            actionTracker.Log("User opened FAQ window.");
            appWindowManager.OpenOrActivateWindow<FaqView, FaqViewModel>();
        }

        public void Quit()
        {
            actionTracker.Log("User quit the app via taskbar menu click.");
            Application.Current.Shutdown();
        }
    }
}
