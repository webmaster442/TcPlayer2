using System;
using System.Collections.Generic;
using System.Linq;

namespace TcPlayer.Engine.Ui
{
    public class Messenger: IMessenger
    {
        private readonly List<Handler> _handlers;

        public Messenger()
        {
            _handlers = new List<Handler>();
        }


        public void SubScribe(IMessageClient subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));

            lock (_handlers)
            {
                if (_handlers.Any(h => h.IsTargetFor(subscriber)))
                    return;

                _handlers.Add(new Handler(subscriber));
            }
        }

        public void UnSubscribe(IMessageClient subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));

            lock (_handlers)
            {
                var h = _handlers.FirstOrDefault(hndl => hndl.IsTargetFor(subscriber));
                if (h != null)
                    _handlers.Remove(h);
            }
        }

        public bool SendMessage(Guid target, object message)
        {
            if (target == Guid.Empty)
                throw new ArgumentException(nameof(target));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            ClearDeadHandlers();

            var targethandler = (from h in _handlers
                                 where h.HasUid(target)
                                 select h).FirstOrDefault();

            if (targethandler == null)
                return false;

            return targethandler.CallHandler(message);
        }

        public bool SendMessage(Type target, Guid id, object message)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (id == Guid.Empty)
                throw new ArgumentException(nameof(id));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            ClearDeadHandlers();

            var targethandler = _handlers.FirstOrDefault(h => h.HasUid(id) && h.IsTypeof(target));

            if (targethandler == null)
                return false;

            return targethandler.CallHandler(message);
        }

        public bool SendMessage(Type targettype, object message)
        {
            if (targettype == null)
                throw new ArgumentNullException(nameof(targettype));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var targethandlers = _handlers.Where(h => h.IsTypeof(targettype));

            var res = true;
            foreach (var target in targethandlers)
            {
                if (!target.CallHandler(message))
                    res = false;

            }
            return res;
        }

        public bool SendMessage(object message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            ClearDeadHandlers();
            var res = true;
            foreach (var h in _handlers)
            {
                if (!h.CallHandler(message))
                    res = false;
            }
            return res;
        }

        private void ClearDeadHandlers()
        {
            Stack <Handler> remove = new Stack<Handler>(_handlers.Where(h => h.IsTargetNull));

            if (remove.Count > 0)
            {
                lock (_handlers)
                {
                    while (remove.Count > 0)
                    {
                        _handlers.Remove(remove.Pop());
                    }
                }
            }
        }
    }
}
