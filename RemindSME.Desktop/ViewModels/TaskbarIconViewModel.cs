using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Net;
using Notifications.Wpf;
using Quobject.SocketIoClientDotNet.Client;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.Views;

//using PowerState = System.Windows.Forms.PowerState;

namespace RemindSME.Desktop.ViewModels
{
    public class TaskbarIconViewModel : PropertyChangedBase
    {
        private const string ServerUrl = "https://reminds-me-server.herokuapp.com/";
        private static readonly TimeSpan HibernationTime = new TimeSpan(18, 00, 00); // 18:00
        private readonly Socket socket;

        private readonly INotificationManager notificationManager;
        private readonly IWindowManager windowManager;

        public TaskbarIconViewModel(INotificationManager notificationManager, IWindowManager windowManager)
        {
            this.notificationManager = notificationManager;
            this.windowManager = windowManager;

            socket = IO.Socket(ServerUrl);
            socket.On("connect", () =>
            {
                var network = NetworkListManager.GetNetworks(NetworkConnectivityLevels.Connected).FirstOrDefault()?.Name;
                socket.Emit("join", network);
            });
            socket.On("last-man-reminder", LastManReminder);

            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

            var timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void OpenHubWindow()
        {
            var existingWindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is HubView);

            if (existingWindow != null)
            {
                if (existingWindow.WindowState == WindowState.Minimized)
                    existingWindow.WindowState = WindowState.Normal;
                existingWindow.Activate();
            }
            else
            {
                var hubViewModel = IoC.Get<HubViewModel>();
                windowManager.ShowWindow(hubViewModel);
            }
        }

        public void SeeNetworkDetails()
        {
            var network = NetworkListManager.GetNetworks(NetworkConnectivityLevels.Connected).FirstOrDefault()?.Name;
            socket.Emit("network", network);
        }

        public void Hibernate()
        {
//            System.Windows.Forms.Application.SetSuspendState(PowerState.Hibernate, false, false);
            MessageBox.Show("Hibernate", "RemindS ME",
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
        }

        public void ShowTestNotification()
        {
            ShowNotification("Test", "Hello world!");
        }

        public void Quit()
        {
            Application.Current.Shutdown();
        }

        private void LastManReminder()
        {
            ShowNotification("Staying a bit later?", "Don't forget to switch out the lights if you're the last one out tonight.");
        }

        public void ShowNotification(string title, string message)
        {
            var model = IoC.Get<NotificationViewModel>();
            model.Title = title;
            model.Message = message;
            notificationManager.Show(model, expirationTime: TimeSpan.FromHours(2));
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    socket.Emit("session-lock");
                    break;
                case SessionSwitchReason.SessionUnlock:
                    socket.Emit("session-unlock");
                    break;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var alreadyHibernatedToday = DateTime.Today <= Settings.Default.LastScheduledHibernate;
            if (alreadyHibernatedToday || DateTime.Now.TimeOfDay < HibernationTime)
            {
                return;
            }
            Settings.Default.LastScheduledHibernate = DateTime.Today;
            Settings.Default.Save();
            Hibernate();
        }
    }
}