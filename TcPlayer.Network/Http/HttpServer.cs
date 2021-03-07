using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcPlayer.Network.Http
{
#pragma warning disable RCS1090 // Add call to 'ConfigureAwait' (or vice versa).
    public sealed class HttpServer : IDisposable
    {
        private TcpListener? _listner;
        private bool _canRun;
        private const int _buffersize = 4096 * 2;
        public Routertable Routes { get; }

        public HttpServer(int port)
        {
            _listner = new TcpListener(IPAddress.Any, port);
            _canRun = true;
            Routes = new Routertable();
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
            using (client)
            {
                using (var stream = client.GetStream())
                {
                    var response = new HttpResponse(stream);
                    try
                    {
                        int readed = 0;
                        var buffer = new byte[_buffersize];
                        HttpRequest? request = null;
                        while (stream.DataAvailable && client.Available < _buffersize && readed < client.Available)
                        {
                            readed = await stream.ReadAsync(buffer, 0, _buffersize);
                            var str = Encoding.UTF8.GetString(buffer, 0, readed);
                            request = new HttpRequest(str);
                        }
                        if (request != null)
                        {
                            var hadndler = Routes.GetHandlerForUrl(request);
                            if (hadndler != null)
                            {
                                await hadndler.Invoke(response);
                            }
                            else
                            {
                                await Handle404(response);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await HandleServerError(response, ex);
                    }
                }
            }

        }

        private async Task HandleServerError(HttpResponse response, Exception ex)
        {
            response.StatusCode = 500;
            response.ContentType = "text/plain";
#if DEBUG
            await response.Write($"{ex.Message}\nTrace:\n{ex.StackTrace}");
#else
            await response.Write("Internal server error");
#endif
        }

        private async Task Handle404(HttpResponse response)
        {
            response.StatusCode = 404;
            response.ContentType = "text/plain";
            await response.Write("not found");
        }
    }
#pragma warning restore RCS1090 // Add call to 'ConfigureAwait' (or vice versa).
}
