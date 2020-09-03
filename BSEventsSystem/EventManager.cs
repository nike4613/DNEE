using System;
using System.Collections.Generic;
using System.Text;

namespace BSEventsSystem
{
    public static class EventManager
    {
        public delegate bool DynamicEventHandler(IEvent @event, dynamic data);
        public delegate bool EventHandler<T>(IEvent<T> @event, in T data);

    }
}
