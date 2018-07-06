using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using Caliburn.Micro;
using Notifications.Wpf;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Helpers;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.ViewModels;
using RemindSME.Desktop.Views;

namespace RemindSME.Desktop.Services
{
    public interface INetworkService : IService
    {
        bool IsWorkNetwork { get; }
        string GetNetworkAddress();
        void AddNetworkFromNotification(bool isWorkNetwork);
        void AddNetwork(bool isWorkNetwork);
    }

    public class NetworkService : INetworkService
    {
        private readonly ILog log;
        private readonly IAppWindowManager appWindowManager;
        private readonly INotificationManager notificationManager;
        private readonly ISettings settings;

        public bool IsWorkNetwork => settings.WorkNetworks.Contains(currentNetwork);
        private string currentNetwork;
        private bool newNetworkNotificationHasBeenShown;

        public NetworkService(
            ILog log,
            IAppWindowManager appWindowManager,
            INotificationManager notificationManager,
            ISettings settings)
        {
            this.log = log;
            this.appWindowManager = appWindowManager;
            this.notificationManager = notificationManager;
            this.settings = settings;
        }

        public void Initialize()
        {
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        }

        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            currentNetwork = GetNetworkAddress();

            if (!newNetworkNotificationHasBeenShown)
            {
                ShowNewNetworkNotification();
            }
        }

        public void AddNetworkFromNotification(bool isWorkNetwork)
        {
            AddNetwork(isWorkNetwork);
            newNetworkNotificationHasBeenShown = false;

            // Resets the bool, ready for the next time it joins a new network.
        }

        public void AddNetwork(bool isWorkNetwork)
        {
            var network = GetNetworkAddress();
            if (isWorkNetwork)
            {
                Settings.Default.WorkNetworks.Add(network);
            }
            else
            {
                Settings.Default.OtherNetworks.Add(network);
            }

            Settings.Default.Save();
        }

        public string GetNetworkAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var addresses = host.AddressList;
            var address = addresses.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            if (address == null)
            {
                return null;
            }

            // Adapted from http://www.java2s.com/Code/CSharp/Network/GetSubnetMask.htm
            var unicastAddresses = NetworkInterface.GetAllNetworkInterfaces().SelectMany(n => n.GetIPProperties().UnicastAddresses);
            var subnetMask = unicastAddresses.FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork && a.Address.Equals(address))?.IPv4Mask;

            var networkAddress = GetNetworkAddress(address, subnetMask);
            return networkAddress.ToString();
        }

        // Adapted from https://blogs.msdn.microsoft.com/knom/2008/12/31/ip-address-calculations-with-c-subnetmasks-networks/
        private static IPAddress GetNetworkAddress(IPAddress address, IPAddress subnetMask)
        {
            if (address == null || subnetMask == null)
            {
                return null;
            }

            var ipAdressBytes = address.GetAddressBytes();
            var subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
            {
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");
            }

            var networkAddress = new byte[ipAdressBytes.Length];
            for (var i = 0; i < networkAddress.Length; i++)
            {
                networkAddress[i] = (byte)(ipAdressBytes[i] & subnetMaskBytes[i]);
            }

            return new IPAddress(networkAddress);
        }

        private void ShowNewNetworkNotification()
        {
            var model = IoC.Get<NewNetworkNotificationViewModel>();
            notificationManager.Show(model, expirationTime: TimeSpan.FromHours(2));
            log.Info("Showed new network notification.");
            newNetworkNotificationHasBeenShown = true;
        }
    }
}
