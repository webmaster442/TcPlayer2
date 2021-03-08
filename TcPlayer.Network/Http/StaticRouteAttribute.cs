using System;
using TcPlayer.Network.Http.Models;

namespace TcPlayer.Network.Http
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class StaticRouteAttribute: Attribute
    {
        public RequestMethod Method { get; }
        public string Location { get; }

        public StaticRouteAttribute(RequestMethod method, string location)
        {
            Method = method;
            Location = location;
        }
    }
}
