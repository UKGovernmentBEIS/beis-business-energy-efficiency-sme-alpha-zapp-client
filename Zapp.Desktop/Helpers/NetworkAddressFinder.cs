using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Zapp.Desktop.Helpers
{
    public interface INetworkAddressFinder
    {
        string GetCurrentNetworkAddress();
    }

    public class NetworkAddressFinder : INetworkAddressFinder
    {
        public string GetCurrentNetworkAddress()
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
