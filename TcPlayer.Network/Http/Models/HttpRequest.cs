// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace TcPlayer.Network.Http.Models
{
    public sealed class HttpRequest
    {
        public RequestMethod Method { get; init; }
        public string Location { get; init; }
        public IDictionary<string, string> Headers { get; init; }
        public IDictionary<string, string> QueryParameters { get; init; }
        public string Content { get; init; }

        public HttpRequest()
        {
            Headers = new Dictionary<string, string>();
            QueryParameters = new Dictionary<string, string>();
            Content = string.Empty;
            Location = string.Empty;
        }
    }
}