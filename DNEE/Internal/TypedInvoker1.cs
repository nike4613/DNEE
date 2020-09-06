using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace DNEE.Internal
{
    internal sealed class TypedInvoker1<T> : IHandlerInvoker<T>
    {
        private readonly TypedHandler1<T> handler;
        private readonly IHandlerInvoker continuation;

        public DataOrigin Origin => handler.Origin;

        public TypedInvoker1(TypedHandler1<T> handler, IHandlerInvoker continueWith)
        {
            this.handler = handler;
            continuation = continueWith;
        }

        public InternalEventResult InvokeWithData(dynamic? data, DataOrigin dataOrigin)
        {
            var obj = (object?)data;
            if (obj is T tval)
                return InvokeWithData(tval, dataOrigin);

            var @event = new TypedInvokedEvent1<T>(dataOrigin, handler.Event, this, obj);

            ExceptionDispatchInfo? caught = null;
            try
            {
                handler.HandlerFunc.Invoke(@event, Maybe.None);
            }
            catch (Exception e)
            {
                caught = ExceptionDispatchInfo.Capture(e);
            }

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                var result = InvokeContinuationDynamic((object?)data, dataOrigin);
                caught = InternalEventResult.CombineExceptions(caught, result.Exception);
            }

            return new InternalEventResult(@event.GetEventResult(), caught);
        }

        public InternalEventResult InvokeWithData(in T data, DataOrigin dataOrigin)
        {
            var @event = new TypedInvokedEvent1<T>(dataOrigin, handler.Event, this, data);

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
                var result = InvokeContinuationTyped(data, dataOrigin);
                caught = InternalEventResult.CombineExceptions(caught, result.Exception);
            }

            return new InternalEventResult(@event.GetEventResult(), caught);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult InvokeContinuationDynamic(dynamic? data, DataOrigin origin)
        {
            return continuation.InvokeWithData((object?)data, origin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult InvokeContinuationTyped(in T data, DataOrigin origin)
        {
            if (continuation is IHandlerInvoker<T> typed)
                return typed.InvokeWithData(data, origin);
            else
                return continuation.InvokeWithData(data, origin);
        }
    }
}
