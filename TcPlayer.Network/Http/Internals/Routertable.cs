using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TcPlayer.Network.Http.Models;

namespace TcPlayer.Network.Http.Internals
{
    public delegate Task RequestHandler(HttpRequest request, HttpResponse response);

    public sealed class Routertable
    {
        internal record Route
        {
            public RequestMethod Method { get; init; }
            public RequestHandler RequestHandler { get; init; }

            public Route()
            {
                RequestHandler = DefaultHandler;
            }

            private Task DefaultHandler(HttpRequest request, HttpResponse response)
            {
                return Task.Delay(0);
            }
        }

        private readonly Dictionary<string, Route> _staticRoutes;
        private readonly Dictionary<Regex, Route> _dynamicRoutes;

        public Routertable()
        {
            _staticRoutes = new Dictionary<string, Route>();
            _dynamicRoutes = new Dictionary<Regex, Route>();
        }

        public void RegisterStaticRoute(string url, RequestMethod method, RequestHandler handler)
        {
            var entry = new Route
            {
                Method = method,
                RequestHandler = handler
            };
            _staticRoutes.Add(url, entry);
        }

        public void RegisterDynamicRoute(Regex pattern, RequestMethod method, RequestHandler handler)
        {
            var entry = new Route
            {
                Method = method,
                RequestHandler = handler
            };
            _dynamicRoutes.Add(pattern, entry);
        }

        internal RequestHandler? GetHandlerForUrl(HttpRequest request)
        {
            if (_staticRoutes.ContainsKey(request.Location)
                && _staticRoutes[request.Location].Method == request.Method)
            {
                return _staticRoutes[request.Location].RequestHandler;
            }
            else
            {
                foreach (var dynamic in _dynamicRoutes)
                {
                    if (dynamic.Key.IsMatch(request.Location)
                        && dynamic.Value.Method == request.Method)
                    {
                        return dynamic.Value.RequestHandler;
                    }
                }
            }
            return null;
        }

    }
}
