using DNEE.Tuning;

namespace DNEE.Internal
{
    internal sealed class EmptyHandlerInvoker : IHandlerInvoker
    {
        public static readonly EmptyHandlerInvoker Invoker = new();

        private EmptyHandlerInvoker() { }

        public InternalEventResult InvokeWithData(dynamic? data, DataOrigin origin, ICreatedEvent? node)
            => default;
    }
}
