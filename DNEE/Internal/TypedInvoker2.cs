using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
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

        public InternalEventResult InvokeWithData(dynamic? data)
        {
            var obj = (object?)data;
            if (obj is T tval)
                return InvokeWithData(tval);

            var @event = new TypedInvokedEvent2<T, R>(this.@event, this, obj);

            ExceptionDispatchInfo? caught = null;
            try
            {
                // TODO: what do I want to do about exceptions?
                handler.HandlerFunc.Invoke(@event, Maybe.None);
            }
            catch (Exception e)
            {
                caught = ExceptionDispatchInfo.Capture(e);
            }

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                var result = InvokeContinuationDynamic((object?)data);
                caught = InternalEventResult.CombineExceptions(caught, result.Exception);
            }

            return new InternalEventResult(@event.GetEventResult(), caught);
        }

        InternalEventResult IHandlerInvoker<T>.InvokeWithData(in T data)
            => InvokeWithData(data);

        public InternalEventResult<R> InvokeWithData(in T data)
        {
            var @event = new TypedInvokedEvent2<T, R>(this.@event, this, data);

            ExceptionDispatchInfo? caught = null;
            try
            {
                handler.HandlerFunc.Invoke(@event, Maybe.Some(data));
            }
            catch (Exception e)
            {
                caught = ExceptionDispatchInfo.Capture(e);
            }

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                var result = InvokeContinuationTyped(data);
                caught = InternalEventResult.CombineExceptions(caught, result.Exception);
            }

            return new InternalEventResult<R>(@event.GetEventResult(), caught);
        }

        internal InternalEventResult<R> InvokeContinuationDynamic(dynamic? data)
        {
            return continuation.InvokeWithData((object?)data);
        }

        internal InternalEventResult<R> InvokeContinuationTyped(in T data)
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
