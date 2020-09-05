namespace DNEE.Internal
{
    internal interface IHandlerInvoker
    {
        InternalEventResult InvokeWithData(dynamic? data);
    }

    internal interface IHandlerInvoker<T> : IHandlerInvoker
    {
        InternalEventResult InvokeWithData(in T data);
    }

    internal interface IHandlerInvoker<T, R> : IHandlerInvoker<T>
    {
        new InternalEventResult<R> InvokeWithData(in T data);
    }
}