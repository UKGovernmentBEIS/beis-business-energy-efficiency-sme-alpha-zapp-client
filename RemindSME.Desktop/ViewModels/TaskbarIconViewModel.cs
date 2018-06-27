using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Net;
using Notifications.Wpf;
using Quobject.SocketIoClientDotNet.Client;
using RemindSME.Desktop.Events;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.Views;
using Squirrel;

using static RemindSME.Desktop.Helpers.HibernationSettings;

//using PowerState = System.Windows.Forms.PowerState;

namespace RemindSME.Desktop.ViewModels
{
    public class TaskbarIconViewModel : PropertyChangedBase, IHandle<NextHibernationTimeUpdatedEvent>
    {
        private const string ServerUrl = "http://localhost:5000";

        private readonly IHibernationManager hibernationManager;
        private readonly INotificationManager notificationManager;
        private readonly IReminderManager reminderManager;
        private readonly IAppUpdateManager updateManager;
        private readonly ISingletonWindowManager singletonWindowManager;

        private bool hibernationPromptHasBeenShown;

        private Socket socket;

        public TaskbarIconViewModel(
            IEventAggregator eventAggregator,
            IHibernationManager hibernationManager,
            INotificationManager notificationManager,
            IReminderManager reminderManager,
            IAppUpdateManager updateManager,
            ISingletonWindowManager singletonWindowManager)
        {
            this.hibernationManager = hibernationManager;
            this.notificationManager = notificationManager;
            this.reminderManager = reminderManager;
            this.updateManager = updateManager;
            this.singletonWindowManager = singletonWindowManager;

            eventAggregator.Subscribe(this);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += Timer_Tick;
            timer.Start();

            var updateTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(5) };
            updateTimer.Tick += UpdateTimer_TickAsync;
            updateTimer.Start();

            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            Connect();
        }

        public void OpenHubWindow()
        {
            singletonWindowManager.OpenOrFocusSingletonWindow<HubView, HubViewModel>();
        }

        public void Quit()
        {
            Application.Current.Shutdown();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
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

        public void ShowHibernationPrompt()
        {
            hibernationPromptHasBeenShown = true;
            var model = IoC.Get<HibernationPromptViewModel>();
            notificationManager.Show(model, expirationTime: TimeSpan.FromHours(2));
        }

        public void Handle(NextHibernationTimeUpdatedEvent message)
        {
            hibernationPromptHasBeenShown = false;
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

        private async void UpdateTimer_TickAsync(object sender, EventArgs e)
        {
            var updateIsAvailable = await updateManager.CheckForUpdate();
            if (updateIsAvailable)
            {
                await updateManager.UpdateAndRestart();
            }
        }

        private void ShowNotification(string title, string message)
        {
            var model = IoC.Get<NotificationViewModel>();
            model.Title = title;
            model.Message = message;
            notificationManager.Show(model, expirationTime: TimeSpan.FromMinutes(5));
        }

        private void Connect()
        {
            if (socket != null)
            {
                return;
            }
            socket = IO.Socket(ServerUrl, new IO.Options { AutoConnect = false });
            socket.On("connect", () =>
            {
                var network = NetworkListManager.GetNetworks(NetworkConnectivityLevels.Connected).FirstOrDefault()?.Name;
                socket.Emit("join", network);
            });
            socket.On("network-count-change", HandleNetworkCountChange);
            socket.Connect();
        }

        private void Disconnect()
        {
            if (socket == null)
            {
                return;
            }
            socket.Disconnect();
            socket = null;
        }

        private void HandleNetworkCountChange(object arg)
        {
            var count = unchecked ((int)(long)arg);
            reminderManager.UpdateNetworkCount(count);
        }
    }
}
