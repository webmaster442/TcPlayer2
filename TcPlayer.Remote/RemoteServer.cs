﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using TcPlayer.Engine;
using WatsonWebserver;

namespace TcPlayer.Remote
{
    public sealed class RemoteServer : IDisposable
    {
        private Server? _server;

        private Guid _sessionId;
        private const int _port = 9000;
        private readonly string _hostName;

        private readonly ConcurrentDictionary<string, string> _cache;
        private readonly IMessenger messenger;

        public RemoteServer(IMessenger messenger)
        {
            _hostName = FirewallHelper.GetLocalIP();
            _sessionId = Guid.NewGuid();
            _cache = new ConcurrentDictionary<string, string>();
            var commandsRegex = new Regex($"^/{_sessionId}/player/(play|pause|stop|next|previous)", RegexOptions.Compiled);
            FillCache();

            _server = new Server(_hostName, _port, false, HandleDefaultRoute);
            _server.Routes.Static.Add(HttpMethod.GET, "app.js", (ctx) => HandleContentFile(ctx, "app.js"));
            _server.Routes.Static.Add(HttpMethod.GET, "style.css", (ctx) => HandleContentFile(ctx, "style.css"));
            _server.Routes.Static.Add(HttpMethod.GET, $"/{_sessionId}/", (ctx) => HandleContentFile(ctx, "index.html"));
            _server.Routes.Dynamic.Add(HttpMethod.GET, commandsRegex, CommandHandler);
            this.messenger = messenger;
        }

        public void Start()
        {
            _server?.Start();
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
                        var key = resourceName.Replace(name + ".Html.", "");
                        var content = reader.ReadToEnd();
                        _cache.TryAdd(key, content);
                    }
                }
            }
        }

        private async Task HandleDefaultRoute(HttpContext ctx)
        {
            ctx.Response.StatusCode = 200;
            await ctx.Response.Send(string.Empty);
        }

        private async Task HandleContentFile(HttpContext ctx, string file)
        {
            if (_cache.ContainsKey(file))
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentType = GetContentType(file);
                await ctx.Response.Send(_cache[file]);
            }
        }

        private async Task CommandHandler(HttpContext ctx)
        {
            var command = ctx.Request.Url.RawWithoutQuery.Replace($"/{_sessionId}/player/", "");

            messenger.SendMessage(new RemoteControlMessage
            {
                Command = Enum.Parse<RemoteControlCommand>(command, true),
                Value = 0
            });

            ctx.Response.StatusCode = 200;
            await ctx.Response.Send("ok");
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
