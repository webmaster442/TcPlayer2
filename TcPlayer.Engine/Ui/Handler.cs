// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TcPlayer.Engine.Ui
{
    internal class Handler
    {
        private readonly WeakReference _wref;
        private readonly Dictionary<Type, MethodInfo> _supported;

        public Handler(IMessageClient o)
        {
            _wref = new WeakReference(o);
            _supported = new Dictionary<Type, MethodInfo>();
            Inspect(o);
        }

        private void Inspect(IMessageClient o)
        {
            var ifaces = o.GetType().GetInterfaces()
                .Where(x => typeof(IMessageClient).IsAssignableFrom(x) && x.IsGenericType);

            foreach (var iface in ifaces)
            {
                var t = iface.GetGenericArguments()[0];
                var m = iface.GetMethod("HandleMessage");

                if (m != null)
                {
                    if (_supported.ContainsKey(t)) _supported[t] = m;
                    else _supported.Add(t, m);
                }
            }

        }

        public bool CallHandler(object param)
        {
            if (param == null)
                throw new ArgumentException(nameof(param));

            if (_wref.Target == null)
                return false;

            var t = param.GetType();

            foreach (var supported in _supported)
            {
                if (supported.Key.IsAssignableFrom(t))
                {
                    supported.Value.Invoke(_wref.Target, new[] { param });
                    return true;
                }
            }
            return false;
        }

        public bool IsTargetNull
        {
            get { return _wref.Target == null; }
        }

        public bool IsTargetFor(IMessageClient o)
        {
            return _wref.Target == o;
        }

        public bool IsTypeof(Type t)
        {
            if (_wref.Target == null)
                return false;
            return _wref.Target.GetType() == t;
        }

        public bool HasUid(Guid search)
        {
            if (_wref.Target == null)
                return false;
            return (_wref.Target as IMessageClient)?.MessageReciverID == search;
        }
    }
}
