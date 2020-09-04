using System;
using System.Collections.Generic;
using System.Text;

namespace BSEventsSystem.Internal
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

        public EventResult InvokeWithData(dynamic? data)
        {
            var @event = new DynamicInvokedEvent(this.@event, this);

            // TODO: what do I want to do about exceptions?
            handler.HandlerFunc.Invoke(@event, (object?)data);

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                InvokeContinuation((object?)data);
            }

            return @event.GetEventResult();
        }

        internal EventResult InvokeContinuation(dynamic? data)
        {
            return continuation.InvokeWithData((object?)data);
        }
    }
}
