using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    internal sealed class TypedInvoker1<T> : IHandlerInvoker<T>
    {
        private readonly EventName @event;
        private readonly TypedHandler1<T> handler;
        private readonly IHandlerInvoker continuation;

        public TypedInvoker1(in EventName name, TypedHandler1<T> handler, IHandlerInvoker continueWith)
        {
            @event = name;
            this.handler = handler;
            continuation = continueWith;
        }

        public EventResult InvokeWithData(dynamic? data)
        {
            var obj = (object?)data;
            if (obj is T tval)
                return InvokeWithData(tval);

            var @event = new TypedInvokedEvent1<T>(this.@event, this, obj);

            // TODO: what do I want to do about exceptions?
            handler.HandlerFunc.Invoke(@event, Maybe.None);

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                InvokeContinuationDynamic((object?)data);
            }

            return @event.GetEventResult();
        }

        public EventResult InvokeWithData(in T data)
        {
            var @event = new TypedInvokedEvent1<T>(this.@event, this, data);

            handler.HandlerFunc.Invoke(@event, Maybe.Some(data));

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                InvokeContinuationTyped(data);
            }

            return @event.GetEventResult();
        }

        internal EventResult InvokeContinuationDynamic(dynamic? data)
        {
            return continuation.InvokeWithData((object?)data);
        }

        internal EventResult InvokeContinuationTyped(in T data)
        {
            if (continuation is IHandlerInvoker<T> typed)
                return typed.InvokeWithData(data);
            else
                return continuation.InvokeWithData(data);
        }
    }
}
