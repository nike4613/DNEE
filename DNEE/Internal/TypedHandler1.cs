using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    internal class TypedHandler1<T> : IHandler
    {
        public EventName Event { get; }
        public EventManager.NoReturnEventHandler<T> HandlerFunc { get; }
        public HandlerPriority Priority { get; }

        public TypedHandler1(in EventName name, EventManager.NoReturnEventHandler<T> func, HandlerPriority priority)
        {
            Event = name;
            HandlerFunc = func;
            Priority = priority;
        }

        public IHandlerInvoker CreateInvokerWithContinuation(IHandlerInvoker continueWith)
        {
            return new TypedInvoker1<T>(Event, this, continueWith);
        }
    }
}
