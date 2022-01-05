using DNEE.Tuning;

namespace DNEE.Internal
{
    internal interface IHandlerInvoker
    {
        InternalEventResult InvokeWithData(dynamic? data, DataOrigin dataOrigin, ICreatedEvent? histNode);
    }

    internal interface IHandlerInvoker<in T> : IHandlerInvoker
    {
        InternalEventResult InvokeWithData(T data, DataOrigin dataOrigin, ICreatedEvent? histNode);
        InternalEventResult InvokeWithRelatedData(object? data, T inputData, DataOrigin dataOrigin, ICreatedEvent? histNode);
    }

    // unfortunately we can't have variance on R
    internal interface IHandlerInvoker<in T, R> : IHandlerInvoker<T>
    {
        new InternalEventResult<R> InvokeWithData(T data, DataOrigin dataOrigin, ICreatedEvent? histNode);
        new InternalEventResult<R> InvokeWithRelatedData(object? data, T inputData, DataOrigin dataOrigin, ICreatedEvent? histNode);
    }
}