using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace TcPlayer.Remote
{
    internal static class FirewallHelper
    {
        public static string GetLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }

        public static void EnableHttpOnLocalNetwork(string listenIP, int port)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = $"netsh http add urlacl url=http://{listenIP}:{port} user={Environment.MachineName}\\{Environment.UserName}",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Verb = "runas"
                }
            };
            process.Start();
            process.WaitForExit();
        }
    }
}
