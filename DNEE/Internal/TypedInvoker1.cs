using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
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

        public InternalEventResult InvokeWithData(dynamic? data)
        {
            var obj = (object?)data;
            if (obj is T tval)
                return InvokeWithData(tval);

            var @event = new TypedInvokedEvent1<T>(this.@event, this, obj);

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

        public InternalEventResult InvokeWithData(in T data)
        {
            var @event = new TypedInvokedEvent1<T>(this.@event, this, data);

            ExceptionDispatchInfo? caught = null;
            try
            {
                // TODO: what do I want to do about exceptions?
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

            return new InternalEventResult(@event.GetEventResult(), caught);
        }

        internal InternalEventResult InvokeContinuationDynamic(dynamic? data)
        {
            return continuation.InvokeWithData((object?)data);
        }

        internal InternalEventResult InvokeContinuationTyped(in T data)
        {
            if (continuation is IHandlerInvoker<T> typed)
                return typed.InvokeWithData(data);
            else
                return continuation.InvokeWithData(data);
        }
    }
}
