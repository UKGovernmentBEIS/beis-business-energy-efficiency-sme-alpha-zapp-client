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
        private const string ServerUrl = "http://localhost:5000";
        private readonly INotificationManager notificationManager;
        private readonly IWindowManager windowManager;

        private Socket socket;
        private bool hibernationPromptHasBeenShown = false;


        public TaskbarIconViewModel(INotificationManager notificationManager, IWindowManager windowManager)
        {
            this.notificationManager = notificationManager;
            this.windowManager = windowManager;

            Connect();
        }


        public void Connect()
        {
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
                {
                    existingWindow.WindowState = WindowState.Normal;
                }
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
            Settings.Default.NextHibernationTime = DateTime.Today;
            Settings.Default.Save();

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

        public void ShowHibernationPrompt()
        {
            var model = IoC.Get<HibernationPromptViewModel>();
            notificationManager.Show(model, expirationTime: TimeSpan.FromHours(2));
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

        public void Lock()
        {
            socket.Disconnect();
        }

        public void Unlock()
        {
            Connect();
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
                    Lock();
                    break;
                case SessionSwitchReason.SessionUnlock:
                    Unlock();
                    break;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            HibernationHelper.HandleHibernationOnTick();
            
            // If your are 15 minutes or less away from the hibernation, prompt the user
            if (Settings.Default.NextHibernationTime.Subtract(DateTime.Now) <= TimeSpan.FromMinutes(15) 
                     && DateTime.Now < Settings.Default.NextHibernationTime)
            {
                if (hibernationPromptHasBeenShown)
                {
                    return;
                }

                ShowHibernationPrompt();
                hibernationPromptHasBeenShown = true;
            }

            //If you are past the hibernation time, then update the next hibernation time then hibernate
            else if (Settings.Default.NextHibernationTime <= DateTime.Now)
            {
                Settings.Default.NextHibernationTime = DateTime.Today.AddDays(1).Add(Settings.Default.DefaultHibernationTime);
                Hibernate();
            }
        }
    }
}
