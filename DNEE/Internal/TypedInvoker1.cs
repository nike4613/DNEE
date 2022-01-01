using DNEE.Utility;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

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

        public InternalEventResult InvokeWithData(dynamic? data, DataOrigin dataOrigin, IDataHistoryNode? histNode)
        {
            var obj = (object?)data;
            if (obj is T tval)
                return InvokeWithData(tval, dataOrigin, histNode);
            if (obj is IUsableAs<T> usable)
                return InvokeWithRelatedData(usable, usable.AsType, dataOrigin, histNode);
            if (obj is IDynamicallyUsableAs dyn && dyn.TryAsType<T>(out var astype))
                return InvokeWithRelatedData(dyn, astype, dataOrigin, histNode);

            var @event = new TypedInvokedEvent1<T>(dataOrigin, handler.Event, this, obj, histNode);

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

        public InternalEventResult InvokeWithData(in T data, DataOrigin dataOrigin, IDataHistoryNode? histNode)
        {
            var @event = new TypedInvokedEvent1<T>(dataOrigin, handler.Event, this, data, histNode);

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

            return new InternalEventResult(@event.GetEventResult(), caught);
        }

        public InternalEventResult InvokeWithRelatedData(object? data, in T inputData, DataOrigin dataOrigin, IDataHistoryNode? histNode)
        {

            var @event = new TypedInvokedEvent1<T>(dataOrigin, handler.Event, this, data, histNode);

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

            return new InternalEventResult(@event.GetEventResult(), caught);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult InvokeContinuationDynamic(dynamic? data, DataOrigin origin, IDataHistoryNode? histNode)
        {
            return continuation.InvokeWithData((object?)data, origin, histNode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult InvokeContinuationTyped(in T data, DataOrigin origin, IDataHistoryNode? histNode)
        {
            if (continuation is IHandlerInvoker<T> typed)
                return typed.InvokeWithData(data, origin, histNode);
            else
                return continuation.InvokeWithData(data, origin, histNode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult InvokeContinuationRelatedTyped(object? data, in T inputData, DataOrigin origin, IDataHistoryNode? histNode)
        {
            if (continuation is IHandlerInvoker<T> typed)
                return typed.InvokeWithRelatedData(data, inputData, origin, histNode);
            else
                return continuation.InvokeWithData(data, origin, histNode);
        }
    }
}
