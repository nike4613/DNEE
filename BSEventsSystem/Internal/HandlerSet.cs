using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSEventsSystem.Internal
{
    internal class HandlerSet
    {
        public static readonly HandlerSet Empty = new();

        private readonly List<IHandler> handlers;

        public IReadOnlyCollection<IHandler> Handlers => handlers;

        public IHandlerInvoker Invoker { get; private set; }

        private HandlerSet()
        {
            handlers = new ();
            Invoker = EmptyHandlerInvoker.Invoker;
        }

        private HandlerSet(HandlerSet copyFrom)
        {
            handlers = new (copyFrom.Handlers);
            Invoker = copyFrom.Invoker;
        }

        public HandlerSet Copy() => new HandlerSet(this);

        // TODO: improve these by implementing custom insertion logic to always insert/remove in the right place
        public void Add(IHandler handler)
        {
            if (handlers.Contains(handler)) return;

            handlers.Add(handler);
            handlers.Sort(HandlerComparer.Instance);
            Invoker = BuildChain(Handlers);
        }

        public void Remove(IHandler handler)
        {
            if (handlers.Remove(handler))
            {
                handlers.Sort(HandlerComparer.Instance);
                Invoker = BuildChain(Handlers);
            }
        }

        private static IHandlerInvoker BuildChain(IReadOnlyCollection<IHandler> handlers)
        {
            if (handlers.Count == 0)
                return EmptyHandlerInvoker.Invoker;

            IHandlerInvoker last = EmptyHandlerInvoker.Invoker;
            foreach (var handler in handlers)
            {
                last = handler.CreateInvokerWithContinuation(last);
            }

            return last;
        }

        // TODO: implement

    }
}
