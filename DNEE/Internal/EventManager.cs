using DNEE.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DNEE.Internal
{
    internal static class EventManager
    {
        internal sealed class HandlerSetCell
        {
            public HandlerSet Handlers = HandlerSet.Empty;
        }

        private static readonly ConcurrentDictionary<EventName, HandlerSetCell> EventHandlers = new();

        private static EventHandle AtomicAddHandler(EventSource source, in EventName @event, IHandler handler)
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

            return new EventHandle(cell, handler, source);
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

        #region Register/Unregister
        internal static EventHandle SubscribeInternal(EventSource source, in EventName @event, DynamicEventHandler handler, HandlerPriority priority)
        {
            return AtomicAddHandler(source, @event, new DynamicHandler(@event, handler, priority));
        }

        internal static EventHandle SubscribeInternal<T>(EventSource source, in EventName @event, NoReturnEventHandler<T> handler, HandlerPriority priority)
        {
            return AtomicAddHandler(source, @event, new TypedHandler1<T>(@event, handler, priority));
        }

        internal static EventHandle SubscribeInternal<T, R>(EventSource source, in EventName @event, ReturnEventHandler<T, R> handler, HandlerPriority priority)
        {
            return AtomicAddHandler(source, @event, new TypedHandler2<T, R>(@event, handler, priority));
        }

        internal static void UnsubscribeInternal(in EventHandle handle)
        {
            // this has the EventSource in the handle
            AtomicRemoveHandler(handle);
        }
        #endregion

        #region Send
        internal static InternalEventResult DynamicSendInternal(EventSource source, in EventName @event, dynamic? data)
        {
            if (!EventHandlers.TryGetValue(@event, out var cell))
                return default; // there are no handlers for the event

            return cell.Handlers.Invoker.InvokeWithData((object?)data);
        }

        internal static InternalEventResult TypedSendInternal<T>(EventSource source, in EventName @event, in T data)
        {
            if (!EventHandlers.TryGetValue(@event, out var cell))
                return default; // there are no handlers for the event

            var invoker = cell.Handlers.Invoker;
            if (invoker is IHandlerInvoker<T> typed)
                return typed.InvokeWithData(data);

            return invoker.InvokeWithData(data);
        }

        internal static InternalEventResult<R> TypedSendInternal<T, R>(EventSource source, in EventName @event, in T data)
        {
            if (!EventHandlers.TryGetValue(@event, out var cell))
                return default; // there are no handlers for the event

            var invoker = cell.Handlers.Invoker;
            if (invoker is IHandlerInvoker<T> typed)
            {
                if (typed is IHandlerInvoker<T, R> typed2)
                    return typed2.InvokeWithData(data);
                return typed.InvokeWithData(data);
            }

            return invoker.InvokeWithData(data);
        }
        #endregion
    }
}
