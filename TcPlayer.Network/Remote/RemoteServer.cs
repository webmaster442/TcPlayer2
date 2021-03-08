using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TcPlayer.Engine;
using TcPlayer.Network.Http;
using TcPlayer.Network.Http.Models;

namespace TcPlayer.Network.Remote
{
    public sealed class RemoteServer : IDisposable
    {
        private const int _port = 9000;
        private readonly Guid _sessionId;
        private readonly string _hostName;

        private readonly ConcurrentDictionary<string, string> _cache;
        private readonly IMessenger _messenger;
        private HttpServer? _httpServer;

        public RemoteServer(IMessenger messenger, ILog serverLog)
        {
            _messenger = messenger;
            _hostName = Dns.GetHostName();
            _sessionId = Guid.NewGuid();
            _cache = new ConcurrentDictionary<string, string>();
            FillCache();
            _httpServer = new HttpServer(serverLog, _port);

            var commandsRegex = new Regex($"^/{_sessionId}/player/(play|pause|stop|next|previous)", RegexOptions.Compiled);
            var filesRegex = new Regex($"^/{_sessionId}(/)|(index\\.html)|(style\\.css)|app(\\.js)|()", RegexOptions.Compiled);
            _httpServer.Routes.RegisterDynamicRoute(filesRegex, RequestMethod.Get, HandleRemoteFiles);
            _httpServer.Routes.RegisterDynamicRoute(commandsRegex, RequestMethod.Get, HandleApiCall);


        }

        public void Start()
        {
            _httpServer?.Start();
        }

        public void Dispose()
        {
            if (_httpServer != null)
            {
                _httpServer.Dispose();
                _httpServer = null;
            }
        }

        public string ServerLink
        {
            get => $"http://{_hostName}:{_port}/{_sessionId}";
        }

        private void FillCache()
        {
            var current = System.Reflection.Assembly.GetAssembly(typeof(RemoteServer));

            var name = current?.GetName().Name ?? string.Empty;

            foreach (var resourceName in current?.GetManifestResourceNames() ?? Enumerable.Empty<string>())
            {
                var stream = current?.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        var key = resourceName.Replace(name + ".RemoteHtml.", "");
                        var content = reader.ReadToEnd();
                        _cache.TryAdd(key, content);
                    }
                }
            }
        }

        private static string GetContentType(string file)
        {
            string extension = System.IO.Path.GetExtension(file).ToLower();
            return extension switch
            {
                ".css" => "text/css",
                ".js" => "text/javascript",
                ".html" => "text/html",
                _ => "text/plain",
            };
        }

        private async Task HandleRemoteFiles(HttpRequest request, HttpResponse response)
        {
            string file = request.Location.Replace($"/{_sessionId}", "");

            if (file.StartsWith("/")) file = file.Substring(1, file.Length - 1);

            if (string.IsNullOrEmpty(file)) file = "index.html";
            if (_cache.ContainsKey(file))
            {
                response.StatusCode = 200;
                response.ContentType = GetContentType(file);
                await response.Write(_cache[file]);
            }
            else
            {
                response.StatusCode = 404;
                response.ContentType = "text/plain";
                await response.Write("not found");
            }
        }

        private async Task HandleApiCall(HttpRequest request, HttpResponse response)
        {
            var command = request.Location.Replace($"/{_sessionId}/player/", "");

            _messenger.SendMessage(new RemoteControlMessage
            {
                Command = Enum.Parse<RemoteControlCommand>(command, true),
            });

            response.StatusCode = 200;
            await response.Write("ok");
        }
    }
}
