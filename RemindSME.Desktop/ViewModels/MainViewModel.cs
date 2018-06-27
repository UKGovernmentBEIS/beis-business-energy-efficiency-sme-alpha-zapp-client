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
using static RemindSME.Desktop.Helpers.HibernationSettings;

//using PowerState = System.Windows.Forms.PowerState;

namespace RemindSME.Desktop.ViewModels
{
    public class MainViewModel : PropertyChangedBase, IHandle<NextHibernationTimeUpdatedEvent>
    {
        private const string ServerUrl = "http://localhost:5000";

        private readonly IHibernationManager hibernationManager;
        private readonly INotificationManager notificationManager;
        private readonly IReminderManager reminderManager;
        private readonly ISingletonWindowManager singletonWindowManager;

        private bool hibernationPromptHasBeenShown;

        private Socket socket;

        public MainViewModel(
            IEventAggregator eventAggregator,
            IHibernationManager hibernationManager,
            INotificationManager notificationManager,
            IReminderManager reminderManager,
            ISingletonWindowManager singletonWindowManager)
        {
            this.hibernationManager = hibernationManager;
            this.notificationManager = notificationManager;
            this.reminderManager = reminderManager;
            this.singletonWindowManager = singletonWindowManager;

            eventAggregator.Subscribe(this);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += Timer_Tick_Reminders;
            timer.Tick += Timer_Tick_Hibernation;
            timer.Start();

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

        private void Timer_Tick_Reminders(object sender, EventArgs e)
        {
            reminderManager.MaybeShowLastManNotification();
        }

        private void Timer_Tick_Hibernation(object sender, EventArgs e)
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
                socket.Emit("join", network, reminderManager.HeatingOptIn);
            });
            socket.On("network-count-change", arg => reminderManager.HandleNetworkCountChange(unchecked((int)(long)arg)));
            socket.On("show-heating-notification",  reminderManager.ShowHeatingNotification);
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
    }
}
