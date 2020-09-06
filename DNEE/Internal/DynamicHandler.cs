using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    internal class DynamicHandler : IHandler
    {
        public EventName Event { get; }
        public DataOrigin Origin { get; }
        public DynamicEventHandler HandlerFunc { get; }
        public HandlerPriority Priority { get; }

        public DynamicHandler(DataOrigin origin, in EventName name, DynamicEventHandler func, HandlerPriority priority)
        {
            Origin = origin;
            Event = name;
            HandlerFunc = func;
            Priority = priority;
        }

        public IHandlerInvoker CreateInvokerWithContinuation(IHandlerInvoker continueWith)
        {
            return new DynamicInvoker(this, continueWith);
        }
    }
}
