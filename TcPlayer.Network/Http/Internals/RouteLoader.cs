using System;
using System.Reflection;
using System.Threading.Tasks;
using TcPlayer.Network.Http.Models;

namespace TcPlayer.Network.Http.Internals
{
    internal static class RouteLoader
    {
        internal static void Load(object objectWithRouteHandlers, Routertable routes)
        {
            if (objectWithRouteHandlers == null)
                throw new ArgumentNullException(nameof(objectWithRouteHandlers));

            if (routes == null)
                throw new ArgumentNullException(nameof(routes));

            var methods = objectWithRouteHandlers
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public);

            foreach (var method in methods)
            {
                StaticRouteAttribute? staticRoute = method.GetCustomAttribute<StaticRouteAttribute>();
                if (staticRoute != null)
                {
                    if (!CheckMethodReturnAndInputParams(method))
                    {
                        throw new InvalidOperationException($"{method.Name} must return a Task and must accept a parameter of {nameof(HttpResponse)}");
                    }
                    routes.RegisterStaticRoute(staticRoute.Location, staticRoute.Method, method.CreateDelegate<RequestHandler>());
                }
                else
                {
                    DynamicRouteAttribute? dynamicRoute = method.GetCustomAttribute<DynamicRouteAttribute>();
                    if (dynamicRoute != null)
                    {
                        if (!CheckMethodReturnAndInputParams(method))
                        {
                            throw new InvalidOperationException($"{method.Name} must return a Task and must accept a parameter of {nameof(HttpResponse)}");
                        }
                        routes.RegisterDynamicRoute(dynamicRoute.Pattern, dynamicRoute.Method, method.CreateDelegate<RequestHandler>());
                    }
                }
            }
        }

        private static bool CheckMethodReturnAndInputParams(MethodInfo method)
        {
            var parameters = method.GetParameters();
            if (parameters.Length != 1) return false;

            return parameters[0].ParameterType == typeof(HttpResponse)
                && !parameters[0].IsOut
                && !parameters[0].IsOptional
                && method.ReturnType == typeof(Task);
        }
    }
}