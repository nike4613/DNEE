using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSEventsSystem.Internal
{
    internal class HandlerSet
    {
        public static readonly HandlerSet Empty = new();

        private readonly SortedSet<IHandler> handlers;

        public IReadOnlyCollection<IHandler> Handlers => handlers;

        public IHandlerInvoker Invoker { get; private set; }

        private HandlerSet()
        {
            handlers = new (HandlerComparer.Instance);
            Invoker = EmptyHandlerInvoker.Invoker;
        }

        private HandlerSet(HandlerSet copyFrom)
        {
            handlers = new (copyFrom.Handlers, HandlerComparer.Instance);
            Invoker = copyFrom.Invoker;
        }

        public HandlerSet Copy() => new HandlerSet(this);

        public void Add(IHandler handler)
        {
            handlers.Add(handler);
            Invoker = BuildChain(Handlers);
        }

        public void Remove(IHandler handler)
        {
            handlers.Remove(handler);
            Invoker = BuildChain(Handlers);
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
