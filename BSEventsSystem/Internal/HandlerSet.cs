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

        public IReadOnlyList<IHandler> Handlers => handlers;

        public IHandlerInvoker Invoker { get; private set; }

        private HandlerSet()
        {
            handlers = new();
            Invoker = EmptyHandlerInvoker.Invoker;
        }

        private HandlerSet(HandlerSet copyFrom)
        {
            handlers = copyFrom.Handlers.ToList();
            Invoker = copyFrom.Invoker;
        }

        public HandlerSet Copy() => new HandlerSet(this);

        internal void Add(IHandler handler)
        {
            throw new NotImplementedException();
        }

        internal void Remove(IHandler handler)
        {
            throw new NotImplementedException();
        }

        // TODO: implement

    }
}
