namespace DNEE.Internal
{
    internal class DynamicHandler : IHandler
    {
        public EventManager Manager { get; }
        public EventName Event { get; }
        public DataOrigin Origin { get; }
        public DynamicEventHandler HandlerFunc { get; }
        public HandlerPriority Priority { get; }

        public DynamicHandler(EventManager manager, DataOrigin origin, in EventName name, DynamicEventHandler func, HandlerPriority priority)
        {
            Manager = manager;
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
