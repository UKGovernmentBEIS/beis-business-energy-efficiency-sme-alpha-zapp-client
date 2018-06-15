﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Net;
using Quobject.SocketIoClientDotNet.Client;
using RemindSME.Desktop.Properties;

//using PowerState = System.Windows.Forms.PowerState;

namespace RemindSME.Desktop.ViewModels
{
    public class TaskbarIconViewModel
    {
        private static readonly TimeSpan HibernationTime = new TimeSpan(18, 00, 00); // 18:00
        private readonly Socket socket;

        public TaskbarIconViewModel()
        {
            var timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Start();

            var network = NetworkListManager.GetNetworks(NetworkConnectivityLevels.Connected).FirstOrDefault()?.Name;
//            socket = IO.Socket("https://reminds-me-server.herokuapp.com/");
            socket = IO.Socket("http://localhost:5000/");
            socket.Emit("join", network);

            Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEventsSessionSwitch);

            void SystemEventsSessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
            {
                if (e.Reason == SessionSwitchReason.SessionLock)
                {
                    socket.Emit("sessionLock");
                    Console.WriteLine(@"locked");
                }
                else if (e.Reason == SessionSwitchReason.SessionUnlock)
                {
                    socket.Emit("sessionUnlock");
                    Console.WriteLine(@"unlocked");
                }
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

        public void Quit()
        {
            Application.Current.Shutdown();
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