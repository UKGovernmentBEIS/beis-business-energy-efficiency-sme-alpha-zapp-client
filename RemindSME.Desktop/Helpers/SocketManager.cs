using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Quobject.SocketIoClientDotNet.Client;
using RemindSME.Desktop.Properties;

namespace RemindSME.Desktop.Helpers
{
    public interface ISocketManager
    {
        Socket Connect();
        void Disconnect();
    }

    public class SocketManager : ISocketManager, IActionTracker
    {
        private static readonly string ServerUrl = ConfigurationManager.AppSettings["ServerUrl"];
        private readonly Queue<string> trackingMessages = new Queue<string>();

        private Socket socket;

        public SocketManager() { }

        public void Log(string message)
        {
            if (socket != null)
            {
                socket.Emit("track", message);
            }
            else
            {
                trackingMessages.Enqueue(message);
            }
        }

        public Socket Connect()
        {
            if (socket != null)
            {
                return socket;
            }
            socket = IO.Socket(ServerUrl, new IO.Options { AutoConnect = false });
            socket.On("connect", () =>
            {
                socket.Emit("join", Settings.Default.CompanyId, Settings.Default.Pseudonym);
                while (trackingMessages.Any())
                {
                    Log(trackingMessages.Dequeue());
                }
            });
            socket.Connect();
            return socket;
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
    }
}
