using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DNEE.Internal
{
    [DebuggerDisplay("Count = {ownHandlers.Count}")]
    internal class HandlerSet
    {
        public static readonly HandlerSet Empty = new();

        // in an ideal world this would be an ImmutableArray or ImmutableList, but that requires another dep
        private readonly List<IHandler> ownHandlers;

        public IHandlerInvoker Invoker { get; }

        private HandlerSet() : this(new(), EmptyHandlerInvoker.Invoker)
        {
        }

        private HandlerSet(List<IHandler> handlers, IHandlerInvoker invoker)
            => (ownHandlers, Invoker) = (handlers, invoker);

        public HandlerSet Add(IHandler handler)
        {
            if (ownHandlers.Contains(handler)) return this;

            var newList = new List<IHandler>(ownHandlers);

            bool inserted = false;

            for (int i = newList.Count - 1; i >= 0; i--)
            {
                if (newList[i].Priority <= handler.Priority)
                {
                    newList.Insert(i + 1, handler);
                    inserted = true;
                    break;
                }
            }

            if (!inserted)
                newList.Insert(0, handler);

            var newInvoker = BuildChain(newList);

            return new HandlerSet(newList, newInvoker);
        }

        public HandlerSet Remove(IHandler handler)
        {
            if (!ownHandlers.Contains(handler)) return this;

            var newList = new List<IHandler>(ownHandlers.Where(h => h != handler));

            var newInvoker = BuildChain(newList);

            return new HandlerSet(newList, newInvoker);
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
