using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    internal class DynamicHandler : IHandler
    {
        public EventName Event { get; }
        public DynamicEventHandler HandlerFunc { get; }
        public HandlerPriority Priority { get; }

        public DynamicHandler(in EventName name, DynamicEventHandler func, HandlerPriority priority)
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
