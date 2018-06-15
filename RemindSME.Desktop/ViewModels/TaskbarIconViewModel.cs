using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Net;
using Quobject.SocketIoClientDotNet.Client;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.Views;

//using PowerState = System.Windows.Forms.PowerState;

namespace RemindSME.Desktop.ViewModels
{
    public class TaskbarIconViewModel : ViewAware
    {
        private const string ServerUrl = "http://localhost:5000";
        private static readonly TimeSpan HibernationTime = new TimeSpan(18, 00, 00); // 18:00
        private readonly Socket socket;

        private readonly IWindowManager windowManager;
        private TaskbarIcon icon;

        public TaskbarIconViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;

            socket = IO.Socket(ServerUrl);
            socket.On("connect", () =>
            {
                var network = NetworkListManager.GetNetworks(NetworkConnectivityLevels.Connected).FirstOrDefault()?.Name;
                socket.Emit("join", network);
            });

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

        public void ShowTestBalloon()
        {
            icon.ShowBalloonTip("Test", "Hello world!", BalloonIcon.Info);
        }

        public void Quit()
        {
            Application.Current.Shutdown();
        }

        protected override void OnViewAttached(object view, object context)
        {
            icon = (view as TaskbarIconView)?.TaskbarIcon;
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
                return;
            Settings.Default.LastScheduledHibernate = DateTime.Today;
            Settings.Default.Save();
            Hibernate();
        }
    }
}