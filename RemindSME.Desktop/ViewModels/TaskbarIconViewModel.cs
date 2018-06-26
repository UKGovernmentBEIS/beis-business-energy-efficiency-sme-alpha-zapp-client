using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Net;
using Notifications.Wpf;
using Quobject.SocketIoClientDotNet.Client;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.Views;

//using PowerState = System.Windows.Forms.PowerState;

namespace RemindSME.Desktop.ViewModels
{
    public class TaskbarIconViewModel : PropertyChangedBase
    {
        private const string ServerUrl = "http://localhost:5000";
        private readonly HibernationManager hibernationManager;
        private readonly INotificationManager notificationManager;
        private readonly IReminderManager reminderManager;
        private readonly ISingletonWindowManager singletonWindowManager;

        private Socket socket;

        public TaskbarIconViewModel(INotificationManager notificationManager, IReminderManager reminderManager, ISingletonWindowManager singletonWindowManager)
        {
            this.notificationManager = notificationManager;
            this.reminderManager = reminderManager;
            this.singletonWindowManager = singletonWindowManager;
            hibernationManager = new HibernationManager();

            var timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Start();

            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            Connect();
        }

        public void OpenHubWindow()
        {
            singletonWindowManager.OpenOrFocusSingletonWindow<HubView, HubViewModel>();
        }

        public void ShowHibernationPrompt()
        {
            var model = IoC.Get<HibernationPromptViewModel>();
            notificationManager.Show(model, expirationTime: TimeSpan.FromHours(2));
        }

        public void Quit()
        {
            Application.Current.Shutdown();
        }

        public void ShowNextHibernationTime()
        {
            MessageBox.Show("The next hibernation time is " + $"{Settings.Default.NextHibernationTime:f}", "RemindS ME",
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    Disconnect();
                    break;
                case SessionSwitchReason.SessionUnlock:
                    Connect();
                    break;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // If before today, set the next hibernation to today at the default time, unless that is within the next 15 minutes,
            // in which case set it for tomorrow.
            if (Settings.Default.NextHibernationTime.Date < DateTime.Today)
            {
                hibernationManager.UpdateNextHiberationTime();
            }

            // If you are past the hibernation time, then hibernate
            if (Settings.Default.NextHibernationTime <= DateTime.Now)
            {
                hibernationManager.Hibernate();
            }

            // If your are 15 minutes or less away from the hibernation, prompt the user
            else if (Settings.Default.NextHibernationTime.Subtract(DateTime.Now) <= TimeSpan.FromMinutes(15))
            {
                if (hibernationManager.HibernationPromptHasBeenShown)
                {
                    return;
                }

                ShowHibernationPrompt();
                hibernationManager.HibernationPromptHasBeenShown = true;
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
