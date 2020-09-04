using BSEventsSystem.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BSEventsSystem
{
    public static partial class EventManager
    {
        internal sealed class HandlerSetCell
        {
            public HandlerSet Handlers = HandlerSet.Empty;
        }

        private static readonly ConcurrentDictionary<EventName, HandlerSetCell> EventHandlers = new();

        private static EventHandle AtomicAddHandler(in EventName @event, IHandler handler)
        {
            var cell = EventHandlers.GetOrAdd(@event, _ => new HandlerSetCell());

            HandlerSet original, handlers;
            do
            {
                original = cell.Handlers;
                handlers = original.Copy();
                handlers.Add(handler);
            }
            while (Interlocked.CompareExchange(ref cell.Handlers, handlers, original) != original);

            return new EventHandle(cell, handler);
        }

        private static void AtomicRemoveHandler(in EventHandle handle)
        {
            var cell = handle.Cell;
            var handler = handle.Handler;

            HandlerSet original, handlers;
            do
            {
                original = cell.Handlers;
                handlers = original.Copy();
                handlers.Remove(handler);
            }
            while (Interlocked.CompareExchange(ref cell.Handlers, handlers, original) != original);
        }

        private sealed class EmptyIHandler : IHandler
        {
            // TODO: remove and replace with actual implementations
            public HandlerPriority Priority => throw new NotImplementedException();

            public EventName Event => throw new NotImplementedException();

            public IHandlerInvoker CreateInvokerWithContinuation(IHandlerInvoker continueWith)
            {
                throw new NotImplementedException();
            }
        }

        #region Register/Unregister
        private static EventHandle RegisterInternal(in EventName @event, DynamicEventHandler handler, HandlerPriority priority)
        {
            // TODO: create actual IHandler for handler
            return AtomicAddHandler(@event, new DynamicHandler(@event, handler, priority));
        }

        private static EventHandle RegisterInternal<T>(in EventName @event, NoReturnEventHandler<T> handler, HandlerPriority priority)
        {
            // TODO: create actual IHandler for handler
            return AtomicAddHandler(@event, new TypedHandler<T>(@event, handler, priority));
        }

        private static EventHandle RegisterInternal<T, R>(in EventName @event, ReturnEventHandler<T, R> handler, HandlerPriority priority)
        {
            // TODO: create actual IHandler for handler
            throw new NotImplementedException();
            return AtomicAddHandler(@event, new EmptyIHandler());
        }

        private static void UnregisterInternal(in EventHandle handle)
        {
            AtomicRemoveHandler(handle);
        }
        #endregion

        #region Send
        private static EventResult DynamicSendInternal(in EventName @event, dynamic? data)
        {
            if (!EventHandlers.TryGetValue(@event, out var cell))
                return default; // there are no handlers for the event

            return cell.Handlers.Invoker.InvokeWithData((object?)data);
        }

        private static EventResult TypedSendInternal<T>(in EventName @event, in T data)
        {
            if (!EventHandlers.TryGetValue(@event, out var cell))
                return default; // there are no handlers for the event

            var invoker = cell.Handlers.Invoker;
            if (invoker is IHandlerInvoker<T> typed)
                return typed.InvokeWithData(data);

            return invoker.InvokeWithData(data);
        }
        
        private static EventResult<R> TypedSendInternal<T, R>(in EventName @event, in T data)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
