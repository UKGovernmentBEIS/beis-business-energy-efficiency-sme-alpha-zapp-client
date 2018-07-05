using System;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Notifications.Wpf;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Events;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.Views;
using Squirrel;
using static RemindSME.Desktop.Helpers.HibernationSettings;

namespace RemindSME.Desktop.ViewModels
{
    public class MainViewModel : PropertyChangedBase, IHandle<NextHibernationTimeUpdatedEvent>
    {
        private readonly IActionTracker actionTracker;
        private readonly IAppWindowManager appWindowManager;
        private readonly ISettings settings;
        private readonly IHibernationManager hibernationManager;
        private readonly INotificationManager notificationManager;
        private readonly IAppUpdateManager updateManager;

        private bool hibernationPromptHasBeenShown;
        private bool hibernationWarningHasBeenShown;

        public MainViewModel(
            IActionTracker actionTracker,
            IAppUpdateManager updateManager,
            IEventAggregator eventAggregator,
            IHibernationManager hibernationManager,
            INotificationManager notificationManager,
            IAppWindowManager appWindowManager,
            ISettings settings)
        {
            this.actionTracker = actionTracker;
            this.hibernationManager = hibernationManager;
            this.notificationManager = notificationManager;
            this.updateManager = updateManager;
            this.appWindowManager = appWindowManager;
            this.settings = settings;

            eventAggregator.Subscribe(this);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += Timer_Tick_Hibernation;
            timer.Start();

            var updateTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(5) };
            updateTimer.Tick += UpdateTimer_TickAsync;
            updateTimer.Start();

            SquirrelAwareApp.HandleEvents(onFirstRun: OpenWelcomeWindow);
        }

        public void Handle(NextHibernationTimeUpdatedEvent e)
        {
            hibernationPromptHasBeenShown = false;
            hibernationWarningHasBeenShown = false;
        }

        public void OpenWelcomeWindow()
        {
            appWindowManager.OpenOrActivateWindow<WelcomeView, WelcomeViewModel>();
        }

        public void OpenHubWindow(string userAction)
        {
            actionTracker.Log($"User opened Hub window via taskbar {userAction}.");
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

        private void Timer_Tick_Hibernation(object sender, EventArgs e)
        {
            if (!settings.HibernationOptIn)
            {
                return;
            }

            var nextHibernationTime = hibernationManager.NextHibernationTime;

            // Next hibernation time is yesterday or earlier, so should be updated.
            if (nextHibernationTime.Date < DateTime.Today)
            {
                hibernationManager.UpdateNextHiberationTime();
            }

            // Within 15 minutes of next hibernation time, so show prompt.
            var timeUntilHibernation = nextHibernationTime.Subtract(DateTime.Now);
            if (!hibernationPromptHasBeenShown && timeUntilHibernation <= HibernationPromptPeriod)
            {
                ShowHibernationPrompt();
            }

            if (!hibernationWarningHasBeenShown && timeUntilHibernation <= HibernationWarningPeriod)
            {
                ShowHibernationWarning();
            }

            // It is time to hibernate!
            if (nextHibernationTime <= DateTime.Now)
            {
                hibernationManager.Hibernate();
            }
        }

        private void ShowHibernationPrompt()
        {
            actionTracker.Log("Showed hibernation prompt.");
            hibernationPromptHasBeenShown = true;
            var model = IoC.Get<HibernationPromptViewModel>();
            notificationManager.Show(model, expirationTime: TimeSpan.FromHours(2));
        }

        private void ShowHibernationWarning()
        {
            actionTracker.Log("Showed hibernation warning.");
            appWindowManager.OpenOrActivateDialog<HibernationWarningView, HibernationWarningViewModel>();
            hibernationWarningHasBeenShown = true;
        }

        private async void UpdateTimer_TickAsync(object sender, EventArgs e)
        {
            var updateIsAvailable = await updateManager.CheckForUpdate();
            if (updateIsAvailable)
            {
                await updateManager.UpdateAndRestart();
            }
        }
    }
}
