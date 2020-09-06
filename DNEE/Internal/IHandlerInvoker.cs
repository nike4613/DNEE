namespace DNEE.Internal
{
    internal interface IHandlerInvoker
    {
        InternalEventResult InvokeWithData(dynamic? data, DataOrigin dataOrigin);
    }

    internal interface IHandlerInvoker<T> : IHandlerInvoker
    {
        InternalEventResult InvokeWithData(in T data, DataOrigin dataOrigin);
    }

    internal interface IHandlerInvoker<T, R> : IHandlerInvoker<T>
    {
        new InternalEventResult<R> InvokeWithData(in T data, DataOrigin dataOrigin);
    }
}