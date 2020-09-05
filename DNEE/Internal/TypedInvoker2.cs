using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    internal sealed class TypedInvoker2<T, R> : IHandlerInvoker<T, R>
    {
        private readonly EventName @event;
        private readonly TypedHandler2<T, R> handler;
        private readonly IHandlerInvoker continuation;

        public TypedInvoker2(in EventName name, TypedHandler2<T, R> handler, IHandlerInvoker continueWith)
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

            var @event = new TypedInvokedEvent2<T, R>(this.@event, this, obj);

            // TODO: what do I want to do about exceptions?
            handler.HandlerFunc.Invoke(@event, Maybe.None);

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                InvokeContinuationDynamic((object?)data);
            }

            return @event.GetEventResult();
        }

        EventResult IHandlerInvoker<T>.InvokeWithData(in T data)
            => InvokeWithData(data);

        public EventResult<R> InvokeWithData(in T data)
        {
            var @event = new TypedInvokedEvent2<T, R>(this.@event, this, data);

            handler.HandlerFunc.Invoke(@event, Maybe.Some(data));

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                InvokeContinuationTyped(data);
            }

            return @event.GetEventResult();
        }

        internal EventResult<R> InvokeContinuationDynamic(dynamic? data)
        {
            return continuation.InvokeWithData((object?)data);
        }

        internal EventResult<R> InvokeContinuationTyped(in T data)
        {
            if (continuation is IHandlerInvoker<T> typed)
            {
                if (typed is IHandlerInvoker<T, R> typed2)
                    return typed2.InvokeWithData(data);
                return typed.InvokeWithData(data);
            }
            else
            {
                return continuation.InvokeWithData(data);
            }
        }
    }
}
