using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Caliburn.Micro;
using Microsoft.Win32;
using Quobject.SocketIoClientDotNet.Client;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Events;
using RemindSME.Desktop.Helpers.Listeners;
using RemindSME.Desktop.Logging;

namespace RemindSME.Desktop.Services
{
    public class SocketManager : IService, IHandle<TrackingEvent>
    {
        private static readonly string ServerUrl = ConfigurationManager.AppSettings["ServerUrl"];
        private readonly CompanyCountChangeListener companyCountChangeListener;

        private readonly HeatingNotificationListener heatingNotificationListener;
        private readonly ISettings settings;

        private readonly Queue<QueuedMessage> trackingMessages = new Queue<QueuedMessage>();

        private Socket socket;

        public SocketManager(
            IEventAggregator eventAggregator,
            ISettings settings,
            CompanyCountChangeListener companyCountChangeListener,
            HeatingNotificationListener heatingNotificationListener)
        {
            this.companyCountChangeListener = companyCountChangeListener;
            this.heatingNotificationListener = heatingNotificationListener;
            this.settings = settings;

            eventAggregator.Subscribe(this);
        }

        public void Handle(TrackingEvent e)
        {
            Log(e.LogLevel, e.Message);
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
                socket.Emit("join", settings.CompanyId, settings.Pseudonym);
                while (trackingMessages.Any())
                {
                    var queuedMessage = trackingMessages.Dequeue();
                    Log(queuedMessage.LogLevel, "{0} (at {1:R})", queuedMessage.Message, queuedMessage.Timestamp);
                }
            });
            socket.On("company-count-change", companyCountChangeListener);
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

        private void Log(LogLevel logLevel, string format, params object[] args)
        {
            var message = string.Format(format, args);
            if (socket != null)
            {
                var level = logLevel.ToString().ToLower();
                socket.Emit($"track-{level}", message);
            }
            else
            {
                trackingMessages.Enqueue(new QueuedMessage(message, logLevel));
            }
        }

        private class QueuedMessage
        {
            internal QueuedMessage(string message, LogLevel logLevel)
            {
                Timestamp = DateTime.Now;
                Message = message;
                LogLevel = logLevel;
            }

            internal DateTime Timestamp { get; }
            internal string Message { get; }
            internal LogLevel LogLevel { get; }
        }
    }
}
