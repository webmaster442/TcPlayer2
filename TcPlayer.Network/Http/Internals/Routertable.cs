// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TcPlayer.Network.Http.Models;

namespace TcPlayer.Network.Http.Internals
{
    public delegate Task RequestHandler(HttpRequest request, HttpResponse response);

    public sealed class Routertable
    {
        internal record Route
        {
            public RequestHandler RequestHandler { get; init; }
            public Predicate<(string url, RequestMethod method)> RouteSelector { get; init; }

            public Route()
            {
                RequestHandler = DefaultHandler;
                RouteSelector = DefaultRouteSelector;
            }

            private bool DefaultRouteSelector((string url, RequestMethod method) obj)
            {
                return false;
            }

            private Task DefaultHandler(HttpRequest request, HttpResponse response)
            {
                return Task.Delay(0);
            }
        }

        private readonly List<Route> _routes;


        public Routertable()
        {
            _routes = new List<Route>();
        }

        public void RegisterRoute(Predicate<(string url, RequestMethod method)> routeSelector,  RequestHandler handler)
        {
            var entry = new Route
            {
                RouteSelector = routeSelector,
                RequestHandler = handler
            };
            _routes.Add(entry);
        }

        internal RequestHandler? GetHandlerForUrl(HttpRequest request)
        {
            foreach (var route in _routes)
            {
                if (route.RouteSelector.Invoke((request.Location, request.Method)))
                {
                    return route.RequestHandler;
                }
            }
            return null;
        }
    }
}
