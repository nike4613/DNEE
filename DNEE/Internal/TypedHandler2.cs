using System.Collections.Generic;

namespace DNEE.Internal
{
    internal class TypedHandler2<T, R> : IHandler
    {
        public EventName Event { get; }
        public ReturnEventHandler<T, R> HandlerFunc { get; }
        public HandlerPriority Priority { get; }
        public DataOrigin Origin { get; }
        public IEnumerable<ITypeConverter> Converters { get; }

        public TypedHandler2(DataOrigin origin, in EventName name, ReturnEventHandler<T, R> func, HandlerPriority priority, IEnumerable<ITypeConverter> converters)
        {
            Origin = origin;
            Event = name;
            HandlerFunc = func;
            Priority = priority;
            Converters = converters;
        }

        public IHandlerInvoker CreateInvokerWithContinuation(IHandlerInvoker continueWith)
        {
            return new TypedInvoker2<T, R>(this, continueWith);
        }
    }
}
