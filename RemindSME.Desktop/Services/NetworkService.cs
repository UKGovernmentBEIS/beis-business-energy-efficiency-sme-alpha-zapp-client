using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using RemindSME.Desktop.Properties;

namespace RemindSME.Desktop.Services
{
    public interface INetworkService : IService 
    {
        string GetNetworkAddress();
        void AddNetwork(bool isWorkNetwork);
    }

    public class NetworkService : INetworkService
    {
        public void Initialize()
        {
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        }

        //public bool IsWorkNetwork => settings.WorkNetworks.Contains(currentNetwork);

        // private string currentNetwork;
        


        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            //update current network
            //fire notification if new network

            Console.WriteLine("New network");

            var network = GetNetworkAddress();
            if (Settings.Default.WorkNetworks.Contains(network))
            {
                Console.WriteLine($@"New network is Work Network");
                // start app as normal
                //do nothing
            }
            else if (Settings.Default.OtherNetworks.Contains(network))
            {
                // don't send any notifications
                // set bool, which can be picked up elsewhere

            }
            else
            {
                // fire new network notification
            }
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
    }
}
