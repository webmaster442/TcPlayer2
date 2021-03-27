// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        private const int _timeout = 2000;
        private readonly ILog _log;
        private (IPAddress addr, IPAddress mask)[] _listenAdresses;

        public Routertable Routes { get; }

        public HttpServer(ILog log, int port)
        {
            _listner = new TcpListener(IPAddress.Any, port);
            _listenAdresses = IPAddressExtensions.GetLocalIpadresses().ToArray();
            _canRun = true;
            Routes = new Routertable();
            _log = log;
        }

#pragma warning disable S3168 // "async" methods should not return "void"
        public async void Start()
#pragma warning restore S3168 // "async" methods should not return "void"
        {
            _listner?.Start();
            while (_canRun && _listner != null)
            {
                try
                {
                    var client = await _listner.AcceptTcpClientAsync();
                    await HandleClient(client);
                }
                catch (Exception ex)
                {
                    _log.Log(ex.Message);
                }
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
                string requestContent = "";
                using (var stream = client.GetStream())
                {
                    var response = new HttpResponse(stream);
                    try
                    {
                        ThrowIfNotOnSameSubnet(client);
                        int readed = 0;
                        var buffer = new byte[_buffersize];
                        int delay = 0;
                        using (var requestSteam = new MemoryStream())
                        {
                            while (client.Available < 1)
                            {
                                delay += 10;
                                await Task.Delay(10);

                                if (delay > _timeout)
                                    throw new InvalidOperationException("Client timeout");
                            }

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

                            requestContent = Encoding.UTF8.GetString(requestSteam.ToArray(), 0, (int)requestSteam.Length);

                            request = RequestParser.ParseRequest(requestContent);
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
                        if (request == null)
                        {
                            _log.Log("Exception on serving: {0}", requestContent ?? "empty request");
                        }
                        else
                        {
                            _log.Log("Exception on serving: {0}", request);
                        }

                        
                        await HandleServerError(response, ex);
                    }
                }
            }

        }

        private void ThrowIfNotOnSameSubnet(TcpClient client)
        {
            var ip = client.GetClientAdress();
            bool allow = false;
            foreach (var listenAdress in _listenAdresses)
            {
                if (listenAdress.addr.IsInSameSubnet(ip, listenAdress.mask))
                {
                    allow = true;
                    break;
                }
            }
            if (!allow)
            {
                throw new InvalidOperationException("Not allowed");
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
