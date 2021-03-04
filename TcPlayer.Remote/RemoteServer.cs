using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace TcPlayer.Remote
{
    public sealed class RemoteServer : IDisposable
    {
        private Server _server;

        private Guid _sessionId;
        private const int _port = 9999;
        private readonly string _hostName;

        private readonly ConcurrentDictionary<string, string> _cache;


        public RemoteServer()
        {
            _hostName = Dns.GetHostName();
            _sessionId = Guid.NewGuid();
            _cache = new ConcurrentDictionary<string, string>();
            FillCache();
            _server = new Server(_hostName, _port, false, HandleDefaultRoute);
            _server.Routes.Static.Add(HttpMethod.GET, "app.js", (ctx) => HandleContent(ctx, "app.js"));
            _server.Routes.Static.Add(HttpMethod.GET, "style.css", (ctx) => HandleContent(ctx, "style.css"));
            _server.Routes.Static.Add(HttpMethod.GET, $"/{_sessionId}/", (ctx) => HandleContent(ctx, "index.html"));
        }

        public string ServerLink
        {
            get => $"http://{_hostName}:{_port}/{_sessionId}";
        }

        private void FillCache()
        {
            var current = System.Reflection.Assembly.GetAssembly(typeof(RemoteServer));
            foreach (var resourceName in current.GetManifestResourceNames())
            {
                using (var reader = new System.IO.StreamReader(current.GetManifestResourceStream(resourceName)))
                {
                    var content = reader.ReadToEnd();
                    _cache.TryAdd(resourceName, content);
                }
            }
        }

        private async Task HandleDefaultRoute(HttpContext ctx)
        {
            ctx.Response.StatusCode = 200;
            await ctx.Response.Send(string.Empty);
        }

        private async Task HandleContent(HttpContext ctx, string file)
        {
            if (_cache.ContainsKey(file))
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentType = GetContentType(file);
                await ctx.Response.Send(_cache[file]);
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

        public void Dispose()
        {
            if (_server != null)
            {
                _server.Dispose();
                _server = null;
            }
        }
    }
}
