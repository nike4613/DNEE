namespace DNEE.Internal
{
    internal interface IHandler
    {
        EventName Event { get; }

        HandlerPriority Priority { get; }

        DataOrigin Origin { get; }

        IHandlerInvoker CreateInvokerWithContinuation(IHandlerInvoker continueWith);
    }
}