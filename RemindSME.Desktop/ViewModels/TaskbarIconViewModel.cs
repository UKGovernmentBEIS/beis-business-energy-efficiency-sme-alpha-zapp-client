﻿using System;
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
        private static readonly TimeSpan HibernationTime = new TimeSpan(18, 00, 00); // 18:00
        private readonly INotificationManager notificationManager;
        private readonly IReminderManager reminderManager;
        private readonly ISingletonWindowManager singletonWindowManager;

        private Socket socket;

        public TaskbarIconViewModel(INotificationManager notificationManager, IReminderManager reminderManager, ISingletonWindowManager singletonWindowManager)
        {
            this.notificationManager = notificationManager;
            this.reminderManager = reminderManager;
            this.singletonWindowManager = singletonWindowManager;

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

        public void Hibernate()
        {
//            System.Windows.Forms.Application.SetSuspendState(PowerState.Hibernate, false, false);
            MessageBox.Show("Hibernate", "RemindS ME",
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
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
