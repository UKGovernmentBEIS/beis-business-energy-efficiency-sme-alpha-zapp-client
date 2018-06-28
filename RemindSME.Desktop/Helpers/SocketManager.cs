using System.Configuration;
using System.Linq;
using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Net;
using Quobject.SocketIoClientDotNet.Client;
using RemindSME.Desktop.Events;

namespace RemindSME.Desktop.Helpers
{
    public interface ISocketManager
    {
        void Connect();
        void Disconnect();
    }

    public class SocketManager : ISocketManager, IHandle<TrackActionEvent>
    {
        private static readonly string ServerUrl = ConfigurationManager.AppSettings["ServerUrl"];

        private readonly IReminderManager reminderManager;
        private readonly string pseudonym;

        private Socket socket;

        public SocketManager(IEventAggregator eventAggregator, IReminderManager reminderManager, string pseudonym)
        {
            this.reminderManager = reminderManager;
            this.pseudonym = pseudonym;

            eventAggregator.Subscribe(this);
        }

        public void Connect()
        {
            if (socket != null)
            {
                return;
            }
            socket = IO.Socket(ServerUrl, new IO.Options { AutoConnect = false });
            socket.On("connect", () =>
            {
                var network = NetworkListManager.GetNetworks(NetworkConnectivityLevels.Connected).FirstOrDefault()?.Name;
                socket.Emit("join", network, pseudonym, reminderManager.HeatingOptIn);
            });
            socket.On("network-count-change", arg => reminderManager.HandleNetworkCountChange(unchecked((int)(long)arg)));
            socket.On("show-heating-notification", reminderManager.ShowHeatingNotificationIfOptedIn);
            socket.Connect();
        }

        public void Disconnect()
        {
            if (socket == null)
            {
                return;
            }
            socket.Disconnect();
            socket = null;
        }

        public void Handle(TrackActionEvent e)
        {
            socket?.Emit("track", e.Message);
        }
    }
}
