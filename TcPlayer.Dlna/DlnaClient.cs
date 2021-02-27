using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
                        if (content.Contains("<netRemote") || content.Contains("<html"))
                        {
                            continue;
                        }

                        var xmlResponse = (Root)xs.Deserialize(reader);
                        var dlna = xmlResponse.Device.ServiceList?.Service.Where(S => S.ServiceType == "urn:schemas-upnp-org:service:ContentDirectory:1").FirstOrDefault();
                        if (dlna != null)
                        {
                            var serverUrl = new Uri(server);
                            var ctrl = $"http://{serverUrl.Host}:{serverUrl.Port}{dlna.ControlURL}";

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

        public static async Task<IReadOnlyList<DlnaItem>> GetContents(string url, string id = "0")
        {
            using (var client = new HttpClient())
            {
                var xml = CreateEnvelope(id);

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Post
                };
                request.Content = new StringContent(xml, Encoding.UTF8, "text/xml");
                request.Headers.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                request.Headers.Add("SOAPAction", "urn:schemas-upnp-org:service:ContentDirectory:1#Browse");

                var response = await client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var xmlResponse = await response.Content.ReadAsStringAsync();
                    using (var reader = new StringReader(HttpUtility.HtmlDecode(xmlResponse)))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Envelope));
                        var envelope = (Envelope)xmlSerializer.Deserialize(reader);
                        return ProcesssBrowseResults(envelope);
                    }
                }

                return Array.Empty<DlnaItem>();
            }
        }

        private static IReadOnlyList<DlnaItem> ProcesssBrowseResults(Envelope envelope)
        {
            List<DlnaItem> items = new List<DlnaItem>();

            if (envelope?.Body?.BrowseResponse?.Result?.DIDLLite?.Items.Any() == true)
            {
                foreach (var objItem in envelope.Body.BrowseResponse.Result.DIDLLite.Items)
                {
                    if (objItem is DIDLLiteItem item)
                    {

                        items.Add(new DlnaItem
                        {
                            Id = item.id,
                            IsBrowsable = false,
                            IsServer = false,
                            Name = item.title,
                        });
                    }
                    else if (objItem is DIDLLiteContainer container)
                    {
                        items.Add(new DlnaItem
                        {
                            Id = container.id,
                            IsBrowsable = true,
                            IsServer = false,
                            Name = container.title,
                        });
                    }
                           
                }
            }

            return items;
        }

        private static string CreateEnvelope(string id = "0")
        {
            var builder = new StringBuilder(512);
            builder.AppendLine("<?xml version=\"1.0\"?>");
            builder.AppendLine("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            builder.AppendLine("<s:Body>");
            builder.AppendLine("<u:Browse xmlns:u=\"urn:schemas-upnp-org:service:ContentDirectory:1\">");
            builder.AppendLine($"<ObjectID>{id}</ObjectID>");
            builder.AppendLine("<BrowseFlag>BrowseDirectChildren</BrowseFlag>");
            builder.AppendLine("<Filter>*</Filter>");
            builder.AppendLine("<StartingIndex>0</StartingIndex>");
            builder.AppendLine("<RequestedCount>0</RequestedCount>");
            builder.AppendLine("<SortCriteria></SortCriteria>");
            builder.AppendLine("</u:Browse>");
            builder.AppendLine("</s:Body>");
            builder.AppendLine("</s:Envelope>");
            return builder.ToString();
        }
    }
}
