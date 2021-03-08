using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace TcPlayer.Network.Http.Models
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
            response.Append("HTTP/1.0 ").Append(StatusCode).AppendLine(" OK");
            response.Append("Content-Length: ").Append(length).AppendLine();
            response.Append("Content-Type: ").AppendLine(ContentType);
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

        internal async Task WriteJson<T>(T input, JsonSerializerOptions? options = null)
        {
            var serialized = JsonSerializer.Serialize(input, options);
            ContentType = "text/json";
            await Write(serialized);
        }
    }
}
