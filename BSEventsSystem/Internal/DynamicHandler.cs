using System;
using System.Collections.Generic;
using System.Text;

namespace BSEventsSystem.Internal
{
    internal class DynamicHandler : IHandler
    {
        public EventName Event { get; }
        public EventManager.DynamicEventHandler HandlerFunc { get; }
        public HandlerPriority Priority { get; }

        public DynamicHandler(in EventName name, EventManager.DynamicEventHandler func, HandlerPriority priority)
        {
            Event = name;
            HandlerFunc = func;
            Priority = priority;
        }

        public IHandlerInvoker CreateInvokerWithContinuation(IHandlerInvoker continueWith)
        {
            return new DynamicInvoker(Event, this, continueWith);
        }
    }
}
