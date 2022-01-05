using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    internal class TypedHandler1<T> : IHandler
    {
        public EventManager Manager { get; }
        public EventName Event { get; }
        public NoReturnEventHandler<T> HandlerFunc { get; }
        public HandlerPriority Priority { get; }
        public DataOrigin Origin { get; }

        public TypedHandler1(EventManager manager, DataOrigin origin, in EventName name, NoReturnEventHandler<T> func, HandlerPriority priority)
        {
            Manager = manager;
            Origin = origin;
            Event = name;
            HandlerFunc = func;
            Priority = priority;
        }

        public IHandlerInvoker CreateInvokerWithContinuation(IHandlerInvoker continueWith)
        {
            return new TypedInvoker1<T>(this, continueWith);
        }
    }
}
