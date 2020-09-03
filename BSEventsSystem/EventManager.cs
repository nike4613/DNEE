﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BSEventsSystem
{
    public static class EventManager
    {
        public delegate bool DynamicEventHandler(IEvent @event, dynamic data);
        public delegate bool NoReturnEventHandler<T>(IEvent<T> @event, in T data);
        public delegate bool ReturnEventHandler<T, R>(IEvent<T, R> @event, in T data);

        public static EventHandle RegisterHandler(in EventName @event, DynamicEventHandler handler, HandlerPriority priority)
        {
            throw new NotImplementedException();
        }

        public static EventHandle RegisterHandler<T>(in EventName @event, EventHandler<T> handler, HandlerPriority priority)
        {
            throw new NotImplementedException();
        }

        public static void UnregisterHandler(in EventHandle handle)
        {
            throw new NotImplementedException();
        }
    }
}
