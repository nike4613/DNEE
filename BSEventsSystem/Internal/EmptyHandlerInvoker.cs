using System;
using System.Collections.Generic;
using System.Text;

namespace BSEventsSystem.Internal
{
    internal sealed class EmptyHandlerInvoker : IHandlerInvoker
    {
        public static readonly EmptyHandlerInvoker Invoker = new();

        public EventResult InvokeWithData(dynamic? data)
            => default;
    }
}
