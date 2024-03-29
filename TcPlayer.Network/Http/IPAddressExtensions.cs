﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace TcPlayer.Network.Http
{
    internal static class IPAddressExtensions
    {
        public static IEnumerable<(IPAddress addr, IPAddress mask)> GetLocalIpadresses()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName(), AddressFamily.InterNetwork);
            bool yielded = false;
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    yielded = true;
                    yield return new(ip, GetSubnetMask(ip));
                }
            }
            if (!yielded)
            {
                yield return
                    (new IPAddress(new byte[] { 127, 0, 0, 1 }),
                     new IPAddress(new byte[] { 255, 0, 0, 0 }));
            }
        }

        private static IPAddress GetSubnetMask(IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork
                        && address.Equals(unicastIPAddressInformation.Address))
                    {
                        return unicastIPAddressInformation.IPv4Mask;
                    }
                }
            }
            throw new ArgumentException($"Can't find subnetmask for IP address '{address}'");
        }

        public static IPAddress GetClientAdress(this TcpClient client)
        {
            if (client.Client.LocalEndPoint is IPEndPoint endPoint
                && endPoint.AddressFamily == AddressFamily.InterNetwork)
            {
                return endPoint.Address;
            }
            throw new ArgumentException("Can't find client adress");
        }

        public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
            {
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");
            }

            var broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            return new IPAddress(broadcastAddress);
        }

        public static bool IsInSameSubnet(this IPAddress address2, IPAddress address, IPAddress subnetMask)
        {
            IPAddress network1 = address.GetNetworkAddress(subnetMask);
            IPAddress network2 = address2.GetNetworkAddress(subnetMask);

            return network1.Equals(network2);
        }
    }
}
