using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace DNEE.Internal
{
    internal sealed class TypedInvoker2<T, R> : IHandlerInvoker<T, R>
    {
        private readonly TypedHandler2<T, R> handler;
        private readonly IHandlerInvoker continuation;

        public DataOrigin Origin => handler.Origin;

        public TypedInvoker2(TypedHandler2<T, R> handler, IHandlerInvoker continueWith)
        {
            this.handler = handler;
            continuation = continueWith;
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification = "Exceptions are caught and automatically propagated using a custom system that minimizes" +
                            "our stack in user stack traces.")]
        public InternalEventResult InvokeWithData(dynamic? data, DataOrigin dataOrigin, IDataHistoryNode? histNode)
        {
            var obj = (object?)data;
            if (obj is T tval)
                return InvokeWithData(tval, dataOrigin, histNode);
            if (obj is IUsableAs<T> usable)
                return InvokeWithUsableData(usable, dataOrigin, histNode);

            var @event = new TypedInvokedEvent2<T, R>(dataOrigin, handler.Event, this, obj, histNode);

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
                var result = InvokeContinuationDynamic((object?)data, dataOrigin, histNode);
                caught = InternalEventResult.CombineExceptions(caught, result.Exception);
            }

            return new InternalEventResult(@event.GetEventResult(), caught);
        }

        InternalEventResult IHandlerInvoker<T>.InvokeWithData(in T data, DataOrigin origin, IDataHistoryNode? histNode)
            => InvokeWithData(data, origin, histNode);


        [SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification = "Exceptions are caught and automatically propagated using a custom system that minimizes" +
                            "our stack in user stack traces.")]
        public InternalEventResult<R> InvokeWithData(in T data, DataOrigin dataOrigin, IDataHistoryNode? histNode)
        {
            var @event = new TypedInvokedEvent2<T, R>(dataOrigin, handler.Event, this, data, histNode);

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
                var result = InvokeContinuationTyped(data, dataOrigin, histNode);
                caught = InternalEventResult.CombineExceptions(caught, result.Exception);
            }

            return new InternalEventResult<R>(@event.GetEventResult(), caught);
        }


        InternalEventResult IHandlerInvoker<T>.InvokeWithUsableData(IUsableAs<T> data, DataOrigin origin, IDataHistoryNode? histNode)
            => InvokeWithUsableData(data, origin, histNode);


        [SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification = "Exceptions are caught and automatically propagated using a custom system that minimizes" +
                            "our stack in user stack traces.")]
        public InternalEventResult<R> InvokeWithUsableData(IUsableAs<T> data, DataOrigin dataOrigin, IDataHistoryNode? histNode)
        {
            var @event = new TypedInvokedEvent2<T, R>(dataOrigin, handler.Event, this, data, histNode);

            ExceptionDispatchInfo? caught = null;
            try
            {
                handler.HandlerFunc.Invoke(@event, Maybe.Some(data.AsType));
            }
            catch (Exception e)
            {
                caught = ExceptionDispatchInfo.Capture(e);
            }

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                var result = InvokeContinuationUsableTyped(data, dataOrigin, histNode);
                caught = InternalEventResult.CombineExceptions(caught, result.Exception);
            }

            return new InternalEventResult<R>(@event.GetEventResult(), caught);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult<R> InvokeContinuationDynamic(dynamic? data, DataOrigin origin, IDataHistoryNode? histNode)
        {
            return continuation.InvokeWithData((object?)data, origin, histNode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult<R> InvokeContinuationTyped(in T data, DataOrigin origin, IDataHistoryNode? histNode)
        {
            if (continuation is IHandlerInvoker<T> typed)
            {
                if (typed is IHandlerInvoker<T, R> typed2)
                    return typed2.InvokeWithData(data, origin, histNode);
                return typed.InvokeWithData(data, origin, histNode);
            }
            else
            {
                return continuation.InvokeWithData(data, origin, histNode);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult<R> InvokeContinuationUsableTyped(IUsableAs<T> data, DataOrigin origin, IDataHistoryNode? histNode)
        {
            if (continuation is IHandlerInvoker<T> typed)
            {
                if (typed is IHandlerInvoker<T, R> typed2)
                    return typed2.InvokeWithUsableData(data, origin, histNode);
                return typed.InvokeWithUsableData(data, origin, histNode);
            }
            else
            {
                return continuation.InvokeWithData(data, origin, histNode);
            }
        }
    }
}
