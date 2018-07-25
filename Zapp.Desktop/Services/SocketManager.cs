using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Caliburn.Micro;
using Microsoft.Win32;
using Quobject.SocketIoClientDotNet.Client;
using Zapp.Desktop.Configuration;
using Zapp.Desktop.Events;
using Zapp.Desktop.Helpers.Listeners;

namespace Zapp.Desktop.Services
{
    public class SocketManager : IService, IHandle<TrackingEvent>, IHandle<NetworkAddressChangedEvent>
    {
        private static readonly string ServerUrl = ConfigurationManager.AppSettings["ServerUrl"];
        private readonly CompanyCountChangeListener companyCountChangeListener;
        private readonly INetworkService networkService;

        private readonly ISettings settings;

        private readonly Queue<QueuedTrackingEvent> trackingMessages = new Queue<QueuedTrackingEvent>();

        private Socket socket;

        public SocketManager(
            IEventAggregator eventAggregator,
            INetworkService networkService,
            ISettings settings,
            CompanyCountChangeListener companyCountChangeListener)
        {
            this.networkService = networkService;
            this.settings = settings;
            this.companyCountChangeListener = companyCountChangeListener;

            eventAggregator.Subscribe(this);
        }

        public void Initialize()
        {
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            if (networkService.IsWorkNetwork)
            {
                Connect();
            }
        }

        public void Handle(NetworkAddressChangedEvent e)
        {
            Disconnect();
            if (e.IsWorkNetwork)
            {
                Connect();
            }
        }

        public void Handle(TrackingEvent e)
        {
            Log(e);
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionUnlock:
                    if (networkService.IsWorkNetwork)
                    {
                        Connect();
                    }
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
                    Log(queuedMessage.TrackingEvent, queuedMessage.Timestamp);
                }
            });
            socket.On("company-count-change", companyCountChangeListener);
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

        private void Log(TrackingEvent e, DateTime? timestamp = null)
        {
            if (socket != null)
            {
                var level = e.LogLevel.ToString().ToLower();
                var message = timestamp.HasValue ? $"{e.Message} (at {timestamp:R})" : e.Message;
                socket.Emit($"track-{level}", message, e.TrackedAction?.ToString());
            }
            else
            {
                trackingMessages.Enqueue(new QueuedTrackingEvent(e));
            }
        }

        private class QueuedTrackingEvent
        {
            internal QueuedTrackingEvent(TrackingEvent trackingEvent)
            {
                Timestamp = DateTime.Now;
                TrackingEvent = trackingEvent;
            }

            internal DateTime Timestamp { get; }
            internal TrackingEvent TrackingEvent { get; }
        }
    }
}
