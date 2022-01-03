namespace DNEE.Internal
{
    internal interface IHandlerInvoker
    {
        InternalEventResult InvokeWithData(dynamic? data, DataOrigin dataOrigin, IDataHistoryNode? histNode);
    }

    internal interface IHandlerInvoker<in T> : IHandlerInvoker
    {
        InternalEventResult InvokeWithData(T data, DataOrigin dataOrigin, IDataHistoryNode? histNode);
        InternalEventResult InvokeWithRelatedData(object? data, T inputData, DataOrigin dataOrigin, IDataHistoryNode? histNode);
    }

    // unfortunately we can't have variance on R
    internal interface IHandlerInvoker<in T, R> : IHandlerInvoker<T>
    {
        new InternalEventResult<R> InvokeWithData(T data, DataOrigin dataOrigin, IDataHistoryNode? histNode);
        new InternalEventResult<R> InvokeWithRelatedData(object? data, T inputData, DataOrigin dataOrigin, IDataHistoryNode? histNode);
    }
}