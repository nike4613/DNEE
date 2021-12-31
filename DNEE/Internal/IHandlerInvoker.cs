namespace DNEE.Internal
{
    internal interface IHandlerInvoker
    {
        InternalEventResult InvokeWithData(dynamic? data, DataOrigin dataOrigin, IDataHistoryNode? histNode);
    }

    internal interface IHandlerInvoker<T> : IHandlerInvoker
    {
        InternalEventResult InvokeWithData(in T data, DataOrigin dataOrigin, IDataHistoryNode? histNode);
        InternalEventResult InvokeWithRelatedData(object? data, in T inputData, DataOrigin dataOrigin, IDataHistoryNode? histNode);
    }

    internal interface IHandlerInvoker<T, R> : IHandlerInvoker<T>
    {
        new InternalEventResult<R> InvokeWithData(in T data, DataOrigin dataOrigin, IDataHistoryNode? histNode);
        new InternalEventResult<R> InvokeWithRelatedData(object? data, in T inputData, DataOrigin dataOrigin, IDataHistoryNode? histNode);
    }
}