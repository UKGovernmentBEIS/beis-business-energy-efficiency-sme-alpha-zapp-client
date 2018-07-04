using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Win32;
using Quobject.SocketIoClientDotNet.Client;
using RemindSME.Desktop.Helpers.Listeners;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.Services;

namespace RemindSME.Desktop.Helpers
{
    public interface ISocketManager : IService { }

    public class SocketManager : ISocketManager, IActionTracker
    {
        private static readonly string ServerUrl = ConfigurationManager.AppSettings["ServerUrl"];

        private readonly HeatingNotificationListener heatingNotificationListener;
        private readonly NetworkCountChangeListener networkCountChangeListener;
        private readonly INetworkFinder networkFinder;

        private readonly Queue<QueuedMessage> trackingMessages = new Queue<QueuedMessage>();

        private Socket socket;

        public SocketManager(
            INetworkFinder networkFinder,
            NetworkCountChangeListener networkCountChangeListener,
            HeatingNotificationListener heatingNotificationListener)
        {
            this.networkFinder = networkFinder;
            this.networkCountChangeListener = networkCountChangeListener;
            this.heatingNotificationListener = heatingNotificationListener;
        }

        public void Log(string message)
        {
            if (socket != null)
            {
                socket.Emit("track", message);
            }
            else
            {
                trackingMessages.Enqueue(new QueuedMessage(message));
            }
        }

        public void Initialize()
        {
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            Connect();
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
                var network = networkFinder.GetNetworkAddress();
                socket.Emit("join", network, Settings.Default.Pseudonym);
                while (trackingMessages.Any())
                {
                    var queuedMessage = trackingMessages.Dequeue();
                    Log($"{queuedMessage.Message} (at {queuedMessage.Timestamp:R})");
                }
            });
            socket.On("network-count-change", networkCountChangeListener);
            socket.On("heating-notification", heatingNotificationListener);
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

        private class QueuedMessage
        {
            internal DateTime Timestamp { get; }
            internal string Message { get; }

            internal QueuedMessage(string message)
            {
                Timestamp = DateTime.Now;
                Message = message;
            }
        }
    }
}
