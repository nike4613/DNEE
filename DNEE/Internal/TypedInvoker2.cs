using DNEE.Utility;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

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

        public InternalEventResult InvokeWithData(dynamic? data, DataOrigin dataOrigin, IDataHistoryNode? histNode)
        {
            var obj = (object?)data;
            if (obj is T tval)
                return InvokeWithData(tval, dataOrigin, histNode);
            if (obj is IUsableAs<T> usable)
                return InvokeWithRelatedData(usable, usable.AsType, dataOrigin, histNode);
            if (obj is IDynamicallyUsableAs dyn && dyn.TryAsType<T>(out var astype))
                return InvokeWithRelatedData(dyn, astype, dataOrigin, histNode);

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

        InternalEventResult IHandlerInvoker<T>.InvokeWithData(T data, DataOrigin origin, IDataHistoryNode? histNode)
            => InvokeWithData(data, origin, histNode);

        public InternalEventResult<R> InvokeWithData(T data, DataOrigin dataOrigin, IDataHistoryNode? histNode)
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


        InternalEventResult IHandlerInvoker<T>.InvokeWithRelatedData(object? data, T inputData, DataOrigin origin, IDataHistoryNode? histNode)
            => InvokeWithRelatedData(data, inputData, origin, histNode);

        public InternalEventResult<R> InvokeWithRelatedData(object? data, T inputData, DataOrigin dataOrigin, IDataHistoryNode? histNode)
        {
            var @event = new TypedInvokedEvent2<T, R>(dataOrigin, handler.Event, this, data, histNode);

            ExceptionDispatchInfo? caught = null;
            try
            {
                handler.HandlerFunc.Invoke(@event, Maybe.Some(inputData));
            }
            catch (Exception e)
            {
                caught = ExceptionDispatchInfo.Capture(e);
            }

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                var result = InvokeContinuationRelatedTyped(data, inputData, dataOrigin, histNode);
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
        internal InternalEventResult<R> InvokeContinuationRelatedTyped(object? data, in T inputData, DataOrigin origin, IDataHistoryNode? histNode)
        {
            if (continuation is IHandlerInvoker<T> typed)
            {
                if (typed is IHandlerInvoker<T, R> typed2)
                    return typed2.InvokeWithRelatedData(data, inputData, origin, histNode);
                return typed.InvokeWithRelatedData(data, inputData, origin, histNode);
            }
            else
            {
                return continuation.InvokeWithData(data, origin, histNode);
            }
        }
    }
}
