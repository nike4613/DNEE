namespace DNEE.Internal
{
    internal class TypedHandler2<T, R> : IHandler
    {
        public EventName Event { get; }
        public ReturnEventHandler<T, R> HandlerFunc { get; }
        public HandlerPriority Priority { get; }

        public TypedHandler2(in EventName name, ReturnEventHandler<T, R> func, HandlerPriority priority)
        {
            Event = name;
            HandlerFunc = func;
            Priority = priority;
        }

        public IHandlerInvoker CreateInvokerWithContinuation(IHandlerInvoker continueWith)
        {
            return new TypedInvoker2<T, R>(Event, this, continueWith);
        }
    }
}
