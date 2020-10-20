using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNEE.Internal
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

        public void Add(IHandler handler)
        {
            if (handlers.Contains(handler)) return;

            bool inserted = false;

            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                if (handlers[i].Priority <= handler.Priority)
                {
                    handlers.Insert(i + 1, handler);
                    inserted = true;
                    break;
                }
            }

            if (!inserted)
                handlers.Insert(0, handler);

            Invoker = BuildChain(Handlers);
        }

        public void Remove(IHandler handler)
        {
            if (handlers.Remove(handler))
            {
                // A removal shouldn't need a resort
                //handlers.Sort(HandlerComparer.Instance);
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

    }
}
