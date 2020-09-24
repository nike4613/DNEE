using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace DNEE.Internal
{
    internal class DynamicInvoker : IHandlerInvoker
    {
        private readonly DynamicHandler handler;
        private readonly IHandlerInvoker continuation;

        public DataOrigin Origin => handler.Origin;

        public DynamicInvoker(DynamicHandler handler, IHandlerInvoker continueWith)
        {
            this.handler = handler;
            continuation = continueWith;
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification = "Exceptions are caught and automatically propagated using a custom system that minimizes" +
                            "our stack in user stack traces.")]
        public InternalEventResult InvokeWithData(dynamic? data, DataOrigin dataOrigin, IDataHistoryNode? lastNode)
        {
            var @event = new DynamicInvokedEvent(dataOrigin, handler.Event, this, (object?)data, lastNode);

            ExceptionDispatchInfo? caught = null;
            try
            {
                handler.HandlerFunc.Invoke(@event, (object?)data);
            }
            catch (Exception e)
            {
                caught = ExceptionDispatchInfo.Capture(e);
            }

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                var result = InvokeContinuation((object?)data, dataOrigin, lastNode);
                caught = InternalEventResult.CombineExceptions(caught, result.Exception);
            }

            return new InternalEventResult(@event.GetEventResult(), caught);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult InvokeContinuation(dynamic? data, DataOrigin origin, IDataHistoryNode? lastNode)
        {
            return continuation.InvokeWithData((object?)data, origin, lastNode);
        }
    }
}
