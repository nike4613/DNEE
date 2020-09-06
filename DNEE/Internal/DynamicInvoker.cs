using System;
using System.Collections.Generic;
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

        public InternalEventResult InvokeWithData(dynamic? data, DataOrigin dataOrigin)
        {
            var @event = new DynamicInvokedEvent(dataOrigin, handler.Event, this);

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
                var result = InvokeContinuation((object?)data, dataOrigin);
                caught = InternalEventResult.CombineExceptions(caught, result.Exception);
            }

            return new InternalEventResult(@event.GetEventResult(), caught);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult InvokeContinuation(dynamic? data, DataOrigin origin)
        {
            return continuation.InvokeWithData((object?)data, origin);
        }
    }
}
