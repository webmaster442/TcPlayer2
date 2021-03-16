// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using TcPlayer.Network.Http.Models;

namespace TcPlayer.Network.Http.Internals
{
    internal static class RequestParser
    {
        private static readonly Regex splitter = new Regex(@"\?|\&");

        public static HttpRequest ParseRequest(string headers)
        {
            var lines = headers.Split('\n');

            if (lines.Length > 0)
            {
                var parts = lines[0].Split(' ');
                var method = Enum.Parse<RequestMethod>(parts[0], true);

                var queryParameters = new Dictionary<string, string>();
                var otherHeaders = new Dictionary<string, string>();

                var location = ExtractLocationAndQueryParams(parts[1], queryParameters);

                var content = ParseOtherHeadersAndContent(parts, otherHeaders);

                return new HttpRequest
                {
                    Location = location,
                    Method = method,
                    QueryParameters = queryParameters,
                    Headers = otherHeaders,
                    Content = content
                };
            }

            return new HttpRequest();
        }

        private static string ExtractLocationAndQueryParams(string rawLocation, Dictionary<string, string> queryParameters)
        {
            rawLocation = HttpUtility.UrlDecode(rawLocation);

            if (rawLocation.Contains('&')
                || rawLocation.Contains('?')
                || rawLocation.Contains('='))
            {
                var parts = splitter.Split(rawLocation);

                for (int i = 1; i < parts.Length; i++)
                {
                    var keyvalue = parts[i].Split('=');
                    if (keyvalue.Length == 2)
                    {
                        queryParameters.Add(keyvalue[0], keyvalue[1]);
                    }
                }
                return parts[0];
            }
            else
            {
                return rawLocation;
            }
        }

        private static string ParseOtherHeadersAndContent(string[] headerLines, Dictionary<string, string> otherHeaders)
        {
            StringBuilder content = new StringBuilder();
            bool contentArea = false;
            for (int i=1; i<headerLines.Length; i++)
            {
                if (!contentArea && string.IsNullOrEmpty(headerLines[i]))
                {
                    contentArea = true;
                    continue;
                }
                if (!contentArea)
                {
                    var keyvalue = headerLines[i].Split(':');
                    if (keyvalue.Length == 2)
                    {
                        otherHeaders.Add(keyvalue[0].Trim(), keyvalue[1].Trim());
                    }
                }
                else
                {
                    content.Append(headerLines[i]);
                    content.Append("\n");
                }
            }

            return content.ToString();
        }
    }
}
