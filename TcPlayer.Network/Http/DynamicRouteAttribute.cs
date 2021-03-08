using System;
using System.Text.RegularExpressions;
using TcPlayer.Network.Http.Models;

namespace TcPlayer.Network.Http
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class DynamicRouteAttribute : Attribute
    {
        public Regex Pattern { get; }
        public RequestMethod Method { get; }

        public DynamicRouteAttribute(RequestMethod method, string pattern)
        {
            Pattern = new Regex(pattern, RegexOptions.Compiled);
            Method = method;
        }
    }
}
