using DNEE.Internal;
using DNEE.Internal.Resources;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DNEE.Internal
{
    internal static class EventManager
    {
        private static readonly ConcurrentDictionary<EventName, HandlerSet> EventHandlers = new();

        private static readonly object InheritanceLock = new();
        //                                           base     -> derived
        private static readonly ConcurrentDictionary<EventName, IEnumerable<EventName>> HandlerInheritance = new();
        //                                           derived  -> base
        private static readonly ConcurrentDictionary<EventName, EventName> HandlerBases = new();

        private static void UpdateBases(in EventName @event, HandlerSet newValue)
        {
            if (HandlerInheritance.TryGetValue(@event, out var derived))
            {
                foreach (var sub in derived)
                {
                    var handlers = EventHandlers.AddOrUpdate(sub, _ => HandlerSet.Empty.Inheriting(newValue), (_, c) => c.Inheriting(newValue));
                    UpdateBases(sub, handlers); // this needs to be recursive to handle longer chains
                }
            }
        }

        private static EventHandle AtomicAddHandler(EventSource source, in EventName @event, IHandler handler)
        {
            var handlers = EventHandlers.AddOrUpdate(@event, _ => HandlerSet.Empty.Add(handler), (_, e) => e.Add(handler));

            // then update derived events, if any
            UpdateBases(@event, handlers);

            return new EventHandle(@event, handler, source.Origin);
        }

        private static bool AtomicRemoveHandler(in EventHandle handle)
        {
            var handler = handle.Handler;
            HandlerSet? foundSet = null;
            var result = EventHandlers.AddOrUpdate(handle.Event, HandlerSet.Empty, (_, e) => (foundSet = e).Remove(handler));
            UpdateBases(handle.Event, result);
            return foundSet != result;
        }

        #region Register/Unregister
        internal static EventHandle SubscribeInternal(EventSource source, in EventName @event, DynamicEventHandler handler, HandlerPriority priority)
        {
            return AtomicAddHandler(source, @event, new DynamicHandler(source.Origin, @event, handler, priority));
        }

        internal static EventHandle SubscribeInternal<T>(EventSource source, in EventName @event, NoReturnEventHandler<T> handler, HandlerPriority priority)
        {
            return AtomicAddHandler(source, @event, new TypedHandler1<T>(source.Origin, @event, handler, priority, source.TypeConverters));
        }

        internal static EventHandle SubscribeInternal<T, R>(EventSource source, in EventName @event, ReturnEventHandler<T, R> handler, HandlerPriority priority)
        {
            return AtomicAddHandler(source, @event, new TypedHandler2<T, R>(source.Origin, @event, handler, priority, source.TypeConverters));
        }

        internal static void UnsubscribeInternal(in EventHandle handle)
        {
            // this has the EventSource in the handle
            if (AtomicRemoveHandler(handle))
                handle.InvokeUnsubEvents();
        }
        #endregion

        #region Inheritance
        internal static void SetBaseInternal(EventSource source, EventName derived, EventName @base)
        {
            if (source.Origin != derived.Origin)
                throw new InvalidOperationException(SR.EventSource_CannotChangeInheritanceOfEvent);

            lock (InheritanceLock)
            {
                // TODO: perform cycle checking

                if (HandlerBases.TryGetValue(derived, out var currentBase))
                {
                    // remove the current derivation
                    HandlerInheritance.AddOrUpdate(currentBase, Enumerable.Empty<EventName>(), (_, c) => new List<EventName>(c.Where(n => n != derived)));
                }

                // add our new base
                HandlerBases.AddOrUpdate(derived, @base, (_, _) => @base);
                HandlerInheritance.AddOrUpdate(@base, new[] { derived }, (_, c) => new List<EventName>(c) { derived });

                if (EventHandlers.TryGetValue(@base, out var baseSet))
                {
                    var newHandler = EventHandlers.AddOrUpdate(derived, _ => HandlerSet.Empty.Inheriting(baseSet), (_, c) => c.Inheriting(baseSet));
                    UpdateBases(derived, newHandler);
                }
            }
        }

        internal static void RemoveBaseInternal(EventSource source, EventName derived)
        {
            if (source.Origin != derived.Origin)
                throw new InvalidOperationException(SR.EventSource_CannotChangeInheritanceOfEvent);

            lock (InheritanceLock)
            {
                if (HandlerBases.TryRemove(derived, out var currentBase))
                {
                    // remove the current derivation
                    HandlerInheritance.AddOrUpdate(currentBase, Enumerable.Empty<EventName>(), (_, c) => new List<EventName>(c.Where(n => n != derived)));
                    var newHandler = EventHandlers.AddOrUpdate(derived, HandlerSet.Empty, (_, c) => c.Inheriting(null));
                    UpdateBases(derived, newHandler);
                }
            }
        }
        #endregion

        #region Send
        internal static InternalEventResult DynamicSendInternal(EventSource source, in EventName @event, dynamic? data, IDataHistoryNode? dataHistory)
        {
            if (!EventHandlers.TryGetValue(@event, out var handlers))
                return default; // there are no handlers for the event

            return handlers.Invoker.InvokeWithData((object?)data, source.Origin, dataHistory);
        }

        internal static InternalEventResult TypedSendInternal<T>(EventSource source, in EventName @event, in T data, IDataHistoryNode? dataHistory)
        {
            if (!EventHandlers.TryGetValue(@event, out var handlers))
                return default; // there are no handlers for the event

            var invoker = handlers.Invoker;
            if (invoker is IHandlerInvoker<T> typed)
                return typed.InvokeWithData(data, source.Origin, dataHistory);

            return invoker.InvokeWithData(data, source.Origin, dataHistory);
        }

        internal static InternalEventResult<R> TypedSendInternal<T, R>(EventSource source, in EventName @event, in T data, IDataHistoryNode? dataHistory)
        {
            if (!EventHandlers.TryGetValue(@event, out var handlers))
                return default; // there are no handlers for the event

            var invoker = handlers.Invoker;
            if (invoker is IHandlerInvoker<T> typed)
            {
                if (typed is IHandlerInvoker<T, R> typed2)
                    return typed2.InvokeWithData(data, source.Origin, dataHistory);
                return typed.InvokeWithData(data, source.Origin, dataHistory);
            }

            return invoker.InvokeWithData(data, source.Origin, dataHistory);
        }
        #endregion
    }
}
