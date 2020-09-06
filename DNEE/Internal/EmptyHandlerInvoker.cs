using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    internal sealed class EmptyHandlerInvoker : IHandlerInvoker
    {
        public static readonly EmptyHandlerInvoker Invoker = new();

        private EmptyHandlerInvoker() { }

        public InternalEventResult InvokeWithData(dynamic? data, DataOrigin origin, IDataHistoryNode? node)
            => default;
    }
}
