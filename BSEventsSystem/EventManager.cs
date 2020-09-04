using BSEventsSystem.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSEventsSystem
{
    public static partial class EventManager
    {
        public delegate void DynamicEventHandler(IEvent @event, dynamic? data);
        public delegate void NoReturnEventHandler<T>(IEvent<T> @event, in Maybe<T> data);
        public delegate void ReturnEventHandler<T, R>(IEvent<T, R> @event, in Maybe<T> data);

        public static EventHandle RegisterHandler(in EventName @event, DynamicEventHandler handler, HandlerPriority priority)
        {
            if (!@event.IsValid)
                throw new ArgumentException("Event name not valid", nameof(@event));
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return RegisterInternal(@event, handler, priority);
        }

        public static EventHandle RegisterHandler<T>(in EventName @event, NoReturnEventHandler<T> handler, HandlerPriority priority)
        {
            if (!@event.IsValid)
                throw new ArgumentException("Event name not valid", nameof(@event));
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return RegisterInternal(@event, handler, priority);
        }

        public static EventHandle RegisterHandler<T, R>(in EventName @event, ReturnEventHandler<T, R> handler, HandlerPriority priority)
        {
            if (!@event.IsValid)
                throw new ArgumentException("Event name not valid", nameof(@event));
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return RegisterInternal(@event, handler, priority);
        }

        public static void UnregisterHandler(in EventHandle handle)
        {
            if (!handle.IsValid)
                throw new ArgumentException("Handle not valid", nameof(handle));

            UnregisterInternal(handle);
        }

        public static EventResult SendEventDynamic(in EventName @event, dynamic? data)
        {
            if (!@event.IsValid)
                throw new ArgumentException("Event name not valid", nameof(@event));

            return DynamicSendInternal(@event, (object?)data);
        }

        public static EventResult SendEvent<T>(in EventName @event, in T data)
        {
            if (!@event.IsValid)
                throw new ArgumentException("Event name not valid", nameof(@event));

            return TypedSendInternal(@event, data);
        }

        public static EventResult<R> SendEvent<T, R>(in EventName @event, in T data)
        {
            if (!@event.IsValid)
                throw new ArgumentException("Event name not valid", nameof(@event));

            return TypedSendInternal<T, R>(@event, data);
        }
    }
}
