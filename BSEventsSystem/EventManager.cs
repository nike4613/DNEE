using BSEventsSystem.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSEventsSystem
{
    public static class EventManager
    {
        public delegate bool DynamicEventHandler(IEvent @event, dynamic data);
        public delegate bool NoReturnEventHandler<T>(IEvent<T> @event, in Maybe<T> data);
        public delegate bool ReturnEventHandler<T, R>(IEvent<T, R> @event, in Maybe<T> data);

        public static EventHandle RegisterHandler(in EventName @event, DynamicEventHandler handler, HandlerPriority priority)
        {
            throw new NotImplementedException();
        }

        public static EventHandle RegisterHandler<T>(in EventName @event, NoReturnEventHandler<T> handler, HandlerPriority priority)
        {
            throw new NotImplementedException();
        }

        public static EventHandle RegisterHandler<T, R>(in EventName @event, ReturnEventHandler<T, R> handler, HandlerPriority priority)
        {
            throw new NotImplementedException();
        }

        public static void UnregisterHandler(in EventHandle handle)
        {
            throw new NotImplementedException();
        }

        public static EventResult SendEventDynamic(in EventName @event, dynamic data)
        {
            throw new NotImplementedException();
        }

        public static EventResult SendEvent<T>(in EventName @event, in T data)
        {
            throw new NotImplementedException();
        }

        public static EventResult<R> SendEvent<T, R>(in EventName @event, in T data)
        {
            throw new NotImplementedException();
        }
    }
}
