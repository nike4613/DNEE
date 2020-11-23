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

        private readonly HandlerSet? inheritFrom;

        private HandlerSet() : this(new(), EmptyHandlerInvoker.Invoker, null)
        {
        }

        private HandlerSet(List<IHandler> handlers, IHandlerInvoker invoker, HandlerSet? inherit)
            => (ownHandlers, Invoker, inheritFrom) = (handlers, invoker, inherit);

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

            var newInvoker = BuildChain(GetHandlers(newList, inheritFrom?.HandlerChain));

            return new HandlerSet(newList, newInvoker, inheritFrom);
        }

        public HandlerSet Remove(IHandler handler)
        {
            if (!ownHandlers.Contains(handler)) return this;

            var newList = new List<IHandler>(ownHandlers.Where(h => h != handler));

            var newInvoker = BuildChain(GetHandlers(newList, inheritFrom?.HandlerChain));

            return new HandlerSet(newList, newInvoker, inheritFrom);
        }

        public HandlerSet Inheriting(HandlerSet? other)
        {
            var newList = new List<IHandler>(ownHandlers);

            var newInvoker = BuildChain(GetHandlers(newList, other?.HandlerChain));

            return new HandlerSet(newList, newInvoker, other);
        }

        private IEnumerable<IHandler> HandlerChain => GetHandlers(ownHandlers, inheritFrom?.HandlerChain);

        // this will either give only ownList, or a sorted enumerable of ownList and otherList (given that they are themselves sorted)
        private static IEnumerable<IHandler> GetHandlers(IEnumerable<IHandler> ownList, IEnumerable<IHandler>? otherList)
        {
            if (otherList is null)
            {
                foreach (var handler in ownList)
                    yield return handler;
                yield break;
            }

            using var a = ownList.GetEnumerator();
            using var b = otherList.GetEnumerator();

            if (!a.MoveNext())
            {
                while (b.MoveNext())
                    yield return b.Current;
                yield break;
            }

            if (!b.MoveNext())
            {
                while (a.MoveNext())
                    yield return a.Current;
                yield break;
            }

            bool an = true, bn = true;
            do
            {
                while ((an && bn && a.Current.Priority < b.Current.Priority)
                    || (an && !bn))
                {
                    yield return a.Current;
                    an = a.MoveNext();
                }

                while ((an && bn && a.Current.Priority >= b.Current.Priority)
                    || (!an && bn))
                {
                    yield return b.Current;
                    bn = b.MoveNext();
                }
            }
            while (an || bn);
        }

        private static IHandlerInvoker BuildChain(IEnumerable<IHandler> handlers)
        {
            if (!handlers.Any())
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
