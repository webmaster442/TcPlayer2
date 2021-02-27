using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TcPlayer.Dlna.Models.Browse;
using TcPlayer.Dlna.Modles.Discovery;

namespace TcPlayer.Dlna
{
    //https://developer.sony.com/develop/audio-control-api/get-started/browse-dlna-file#tutorial-step-2
    public static class DlnaClient
    {
        private static async IAsyncEnumerable<string> SSDP()
        {
            IPEndPoint LocalEndPoint = new IPEndPoint(IPAddress.Any, 6000);
            IPEndPoint MulticastEndPoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);//SSDP port
            using (Socket UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                UdpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                UdpSocket.Bind(LocalEndPoint);
                UdpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(MulticastEndPoint.Address, IPAddress.Any));
                UdpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
                UdpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);
                string SearchString = "M-SEARCH * HTTP/1.1\r\nHOST:239.255.255.250:1900\r\nMAN:\"ssdp:discover\"\r\nST:ssdp:all\r\nMX:3\r\n\r\n";
                UdpSocket.SendTo(Encoding.UTF8.GetBytes(SearchString), SocketFlags.None, MulticastEndPoint);
                byte[] ReceiveBuffer = new byte[4000];
                int Count = 0;
                while (Count < 20)
                {
                    Count++;
                    if (UdpSocket.Available > 0)
                    {
                        int ReceivedBytes = UdpSocket.Receive(ReceiveBuffer, SocketFlags.None);
                        if (ReceivedBytes > 0)
                        {
                            string Data = Encoding.UTF8.GetString(ReceiveBuffer, 0, ReceivedBytes);
                            if (Data.ToUpper().IndexOf("LOCATION: ") > -1)
                            {
                                Data = Data.ChopOffBefore("LOCATION: ").ChopOffAfter(Environment.NewLine);
                            }
                            yield return Data;
                        }
                    }
                    else
                    {
                        await Task.Delay(100);
                    }
                }
            }
        }

        public static async Task<IReadOnlyList<DlnaItem>> GetServers()
        {
            var dlnaServers = new HashSet<DlnaItem>();
            await foreach (var server in SSDP())
            {
                XmlSerializer xs = new XmlSerializer(typeof(Root));
                using (var wc = new WebClient())
                {
                    var content = await wc.DownloadStringTaskAsync(server);
                    using (var reader = new StringReader(content))
                    {
                        var xmlResponse = (Root)xs.Deserialize(reader);
                        var dlna = xmlResponse.Device.ServiceList?.Service.Where(S => S.ServiceType == "urn:schemas-upnp-org:service:ContentDirectory:1").FirstOrDefault();
                        if (dlna != null)
                        {
                            var serverUrl = new Uri(server);
                            var ctrl = $"http://{serverUrl.Host}{dlna.ControlURL}";

                            dlnaServers.Add(new DlnaItem
                            {
                                IsBrowsable = true,
                                IsServer = true,
                                Name = xmlResponse.Device.FriendlyName,
                                Locaction = ctrl,
                            });
                        }
                    }
                }
            }
            return dlnaServers.ToList();
        }

        public static async Task GetContents(string url, int id = 0)
        {
            using (var client = new HttpClient())
            {
                var content = new ByteArrayContent(CreateEnvelope(id));
                content.Headers.ContentType = new MediaTypeHeaderValue("text/xml; charset=utf-8");
                content.Headers.Add("SOAPAction", "urn:schemas-upnp-org:service:ContentDirectory:1#Browse");
                var response = await client.PostAsync(url, content);

            }
        }

        private static byte[] CreateEnvelope(int id = 0)
        {
            var serializer = new XmlSerializer(typeof(Envelope));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("s", "http://schemas.xmlsoap.org/soap/envelope/");
            namespaces.Add("u", "urn:schemas-upnp-org:service:ContentDirectory:1");

            Envelope e = new Envelope
            {
                Body = new Body
                {
                    Browse = new Browse
                    {
                        ObjectID = id,
                    }
                }
            };
            using (var memoryStream = new MemoryStream(1024))
            {
                serializer.Serialize(memoryStream, e, namespaces);
                return memoryStream.ToArray();
            }
        }
    }
}
