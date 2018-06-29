using System;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Microsoft.Win32;
using Notifications.Wpf;
using RemindSME.Desktop.Events;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Helpers.Listeners;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.Views;
using Squirrel;
using static RemindSME.Desktop.Helpers.HibernationSettings;

namespace RemindSME.Desktop.ViewModels
{
    public class MainViewModel : PropertyChangedBase, IHandle<NextHibernationTimeUpdatedEvent>
    {
        private readonly IActionTracker actionTracker;
        private readonly IHibernationManager hibernationManager;
        private readonly INotificationManager notificationManager;
        private readonly IReminderManager reminderManager;
        private readonly ISingletonWindowManager singletonWindowManager;
        private readonly ISocketManager socketManager;
        private readonly IAppUpdateManager updateManager;

        private bool hibernationPromptHasBeenShown;

        public MainViewModel(
            IActionTracker actionTracker,
            IAppUpdateManager updateManager,
            IEventAggregator eventAggregator,
            IHibernationManager hibernationManager,
            INotificationManager notificationManager,
            IReminderManager reminderManager,
            ISingletonWindowManager singletonWindowManager,
            ISocketManager socketManager)
        {
            this.actionTracker = actionTracker;
            this.hibernationManager = hibernationManager;
            this.notificationManager = notificationManager;
            this.reminderManager = reminderManager;
            this.updateManager = updateManager;
            this.singletonWindowManager = singletonWindowManager;
            this.socketManager = socketManager;

            eventAggregator.Subscribe(this);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += Timer_Tick_Reminders;
            timer.Tick += Timer_Tick_Hibernation;
            timer.Start();

            var updateTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(5) };
            updateTimer.Tick += UpdateTimer_TickAsync;
            updateTimer.Start();

            SquirrelAwareApp.HandleEvents(onFirstRun: OpenWelcomeWindow);

            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            Connect();
        }

        public void Handle(NextHibernationTimeUpdatedEvent e)
        {
            hibernationPromptHasBeenShown = false;
        }

        public void OpenWelcomeWindow()
        {
            singletonWindowManager.OpenOrActivateSingletonWindow<WelcomeView, WelcomeViewModel>();
        }

        public void OpenHubWindow(string userAction)
        {
            actionTracker.Log($"User opened Hub window via taskbar {userAction}.");
            OpenHubWindow();
        }

        public void OpenHubWindow()
        {
            singletonWindowManager.OpenOrActivateSingletonWindow<HubView, HubViewModel>();
        }

        public void Quit()
        {
            actionTracker.Log("User quit the app via taskbar menu click.");
            Application.Current.Shutdown();
        }

        private void Timer_Tick_Reminders(object sender, EventArgs e)
        {
            reminderManager.MaybeShowTimeDependentNotifications();
        }

        private void Timer_Tick_Hibernation(object sender, EventArgs e)
        {
            if (!Settings.Default.HibernationOptIn)
            {
                return;
            }

            var nextHibernationTime = Settings.Default.NextHibernationTime;

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

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionUnlock:
                    Connect();
                    break;
                case SessionSwitchReason.SessionLock:
                    Disconnect();
                    break;
            }
        }

        private void Connect()
        {
            var socket = socketManager.Connect();
            socket.On("network-count-change", new NetworkCountChangeListener(reminderManager));
            socket.On("heating-notification", new HeatingNotificationListener(reminderManager));
        }

        private void Disconnect()
        {
            socketManager.Disconnect();
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
