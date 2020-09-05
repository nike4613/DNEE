using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;

namespace DNEE.Internal
{
    internal class DynamicInvoker : IHandlerInvoker
    {
        private readonly EventName @event;
        private readonly DynamicHandler handler;
        private readonly IHandlerInvoker continuation;

        public DynamicInvoker(in EventName name, DynamicHandler handler, IHandlerInvoker continueWith)
        {
            @event = name;
            this.handler = handler;
            continuation = continueWith;
        }

        public InternalEventResult InvokeWithData(dynamic? data)
        {
            var @event = new DynamicInvokedEvent(this.@event, this);

            ExceptionDispatchInfo? caught = null;
            try
            {
                // TODO: what do I want to do about exceptions?
                handler.HandlerFunc.Invoke(@event, (object?)data);
            }
            catch (Exception e)
            {
                caught = ExceptionDispatchInfo.Capture(e);
            }

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                var result = InvokeContinuation((object?)data);
                if (result.Exception != null)
                {
                    if (caught != null)
                    {
                        caught = ExceptionDispatchInfo.Capture(new AggregateException(caught.SourceException, result.Exception.SourceException).Flatten());
                    }
                    else
                    {
                        caught = result.Exception;
                    }
                }
            }

            return new InternalEventResult(@event.GetEventResult(), caught);
        }

        internal InternalEventResult InvokeContinuation(dynamic? data)
        {
            return continuation.InvokeWithData((object?)data);
        }
    }
}
