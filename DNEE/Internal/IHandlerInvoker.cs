namespace DNEE.Internal
{
    internal interface IHandlerInvoker
    {
        EventResult InvokeWithData(dynamic? data);
    }

    internal interface IHandlerInvoker<T> : IHandlerInvoker
    {
        EventResult InvokeWithData(in T data);
    }

    internal interface IHandlerInvoker<T, R> : IHandlerInvoker<T>
    {
        new EventResult<R> InvokeWithData(in T data);
    }
}