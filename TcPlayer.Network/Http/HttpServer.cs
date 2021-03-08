using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TcPlayer.Network.Http.Internals;
using TcPlayer.Network.Http.Models;

namespace TcPlayer.Network.Http
{
#pragma warning disable RCS1090 // Add call to 'ConfigureAwait' (or vice versa).
    public sealed class HttpServer : IDisposable
    {
        private TcpListener? _listner;
        private bool _canRun;
        private const int _buffersize = 4096 * 2; //8kiB
        private const int _maxRequestSize = 1024 * 1024 * 4; //4MiB
        private readonly ILog _log;

        public Routertable Routes { get; }

        public HttpServer(ILog log, int port)
        {
            _listner = new TcpListener(IPAddress.Any, port);
            _canRun = true;
            Routes = new Routertable();
            _log = log;
        }

        public void LoadRoutes(object objectWithRouteHandlers)
        {
            RouteLoader.Load(objectWithRouteHandlers, Routes);
        }

#pragma warning disable S3168 // "async" methods should not return "void"
        public async void Start()
#pragma warning restore S3168 // "async" methods should not return "void"
        {
            _listner?.Start();
            while (_canRun && _listner != null)
            {
                var client = await _listner.AcceptTcpClientAsync();
                await HandleClient(client);
            }
        }

        public void Stop()
        {
            _canRun = false;
            _listner?.Stop();
        }

        public void Dispose()
        {
            Stop();
            _listner = null;
        }

        private async Task HandleClient(TcpClient client)
        {
            await Task.Yield();
            int totalSize = 0;
            using (client)
            {
                HttpRequest? request = null;
                using (var stream = client.GetStream())
                {
                    var response = new HttpResponse(stream);
                    try
                    {
                        int readed = 0;
                        var buffer = new byte[_buffersize];

                        using (var requestSteam = new MemoryStream())
                        {
                            while (stream.DataAvailable && client.Available > 0)
                            {
                                readed = await stream.ReadAsync(buffer.AsMemory(0, _buffersize));
                                totalSize += readed;
                                if (totalSize > _maxRequestSize)
                                {
                                    throw new InvalidOperationException("Too big header");
                                }
                                await requestSteam.WriteAsync(buffer.AsMemory(0, readed));
                            }

                            request = RequestParser.ParseRequest(requestSteam);
                        }
                        if (request != null)
                        {
                            var hadndler = Routes.GetHandlerForUrl(request);
                            if (hadndler != null)
                            {
                                _log.Log("Handling: {0}", request?.Location ?? "unknown location");
                                await hadndler.Invoke(request!, response);
                            }
                            else
                            {
                                _log.Log("Not found: {0}", request?.Location ?? "unknown location");
                                await Handle404(response);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Log("Exception on serving: {0}", request?.Location ?? "unknown location");
                        await HandleServerError(response, ex);
                    }
                }
            }

        }

        private static async Task HandleServerError(HttpResponse response, Exception ex)
        {
            response.StatusCode = 500;
            response.ContentType = "text/plain";
#if DEBUG
            await response.Write($"{ex.Message}\nTrace:\n{ex.StackTrace}");
#else
            await response.Write("Internal server error");
#endif
        }

        private static async Task Handle404(HttpResponse response)
        {
            response.StatusCode = 404;
            response.ContentType = "text/plain";
            await response.Write("not found");
        }
    }
#pragma warning restore RCS1090 // Add call to 'ConfigureAwait' (or vice versa).
}
