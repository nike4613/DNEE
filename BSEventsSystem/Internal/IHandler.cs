namespace BSEventsSystem.Internal
{
    internal interface IHandler
    {

        // TODO: implement
        EventName Event { get; }

        HandlerPriority Priority { get; }

        IHandlerInvoker CreateInvokerWithContinuation(IHandlerInvoker continueWith);
    }
}