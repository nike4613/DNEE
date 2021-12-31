using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

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
