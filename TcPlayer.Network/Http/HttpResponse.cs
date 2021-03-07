using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcPlayer.Network.Http
{
    public sealed class HttpResponse : IDisposable
    {
        private NetworkStream? stream;

        public HttpResponse(NetworkStream stream)
        {
            this.stream = stream;
            ContentType = "text/plain";
        }

        public int StatusCode { get; set; }
        public string ContentType { get; set; }

        public void Dispose()
        {
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
        }

        private string CreateHeaders(int length)
        {
            StringBuilder response = new StringBuilder();
            response.AppendLine($"HTTP/1.0 {StatusCode} OK");
            response.AppendLine($"Content-Length: {length}");
            response.AppendLine($"Content-Type: {ContentType}");
            response.AppendLine();
            return response.ToString();
        }

        internal async Task Write(string text)
        {
            var txt = Encoding.UTF8.GetBytes(text);
            var headers = Encoding.UTF8.GetBytes(CreateHeaders(txt.Length));
            var end = Encoding.UTF8.GetBytes("\n\n");
            if (stream != null)
            {
#pragma warning disable RCS1090 // Add call to 'ConfigureAwait' (or vice versa).
                await stream.WriteAsync(headers);
                await stream.WriteAsync(txt);
                await stream.WriteAsync(end);
#pragma warning restore RCS1090 // Add call to 'ConfigureAwait' (or vice versa).
            }

        }
    }
}
