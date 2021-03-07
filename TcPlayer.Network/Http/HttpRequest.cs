using System;
using System.Collections.Generic;

namespace TcPlayer.Network.Http
{
    internal class HttpRequest
    {
        public Method Method { get; init; }
        public string Location { get; init; }

        public IDictionary<string, string> Headers { get; init; }

        public HttpRequest(string str)
        {
            Headers = new Dictionary<string, string>();
            Location = string.Empty;
            var lines = str.Split('\n');
            if (lines.Length > 0)
            {
                var parts = lines[0].Split(' ');
                Method = Enum.Parse<Method>(parts[0], true);
                Location = parts[1];

                ParseOtherHeaders(lines);
            }
        }

        private void ParseOtherHeaders(string[] lines)
        {
            for (int i=1; i<lines.Length; i++)
            {
                var keyvalue = lines[i].Split(':');
                if (keyvalue.Length == 2)
                {
                    Headers.Add(keyvalue[0].Trim(), keyvalue[1].Trim());
                }
            }
        }
    }
}
