using System.Collections.Generic;

namespace DNEE.Internal
{
    internal class TypedHandler2<T, R> : IHandler
    {
        public EventManager Manager { get; }
        public EventName Event { get; }
        public ReturnEventHandler<T, R> HandlerFunc { get; }
        public HandlerPriority Priority { get; }
        public DataOrigin Origin { get; }

        public TypedHandler2(EventManager manager, DataOrigin origin, in EventName name, ReturnEventHandler<T, R> func, HandlerPriority priority)
        {
            Manager = manager;
            Origin = origin;
            Event = name;
            HandlerFunc = func;
            Priority = priority;
        }

        public IHandlerInvoker CreateInvokerWithContinuation(IHandlerInvoker continueWith)
        {
            return new TypedInvoker2<T, R>(this, continueWith);
        }
    }
}
