using DNEE.Tuning;
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

        public InternalEventResult InvokeWithData(dynamic? data, DataOrigin dataOrigin, ICreatedEvent? histNode)
        {
            var obj = (object?)data;
            if (obj is T tval)
                return InvokeWithData(tval, dataOrigin, histNode);
            if (Helpers.TryUseAs<T>((object?)data, out var astype))
                return InvokeWithRelatedData(data, astype, dataOrigin, histNode);

            TypedInvokedEvent2<T, R> @event;
            ExceptionDispatchInfo? caught = null;

            using (var allocated = handler.Manager.Allocator
                .AllocateInOutTyped<TypedInvokedEvent2<T, R>, T, R>(new(dataOrigin, handler.Event, this, obj, histNode)))
            {
                try
                {
                    handler.HandlerFunc.Invoke(allocated.Object, Maybe.None);
                }
                catch (Exception e)
                {
                    caught = ExceptionDispatchInfo.Capture(e);
                }

                // at this point we don't need to keep it allocated, we can copy out of the heap 
                @event = allocated.Object.Reset();
            }

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                var result = InvokeContinuationDynamic((object?)data, dataOrigin, histNode);
                caught = InternalEventResult.CombineExceptions(caught, result.Exception);
            }

            return new InternalEventResult(@event.GetEventResult(), caught);
        }

        InternalEventResult IHandlerInvoker<T>.InvokeWithData(T data, DataOrigin origin, ICreatedEvent? histNode)
            => InvokeWithData(data, origin, histNode);

        public InternalEventResult<R> InvokeWithData(T data, DataOrigin dataOrigin, ICreatedEvent? histNode)
        {
            TypedInvokedEvent2<T, R> @event;
            ExceptionDispatchInfo? caught = null;

            using (var allocated = handler.Manager.Allocator
                .AllocateInOutTyped<TypedInvokedEvent2<T, R>, T, R>(new(dataOrigin, handler.Event, this, data, histNode)))
            {
                try
                {
                    handler.HandlerFunc.Invoke(allocated.Object, Maybe.Some(data));
                }
                catch (Exception e)
                {
                    caught = ExceptionDispatchInfo.Capture(e);
                }

                // at this point we don't need to keep it allocated, we can copy out of the heap 
                @event = allocated.Object.Reset();
            }

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                var result = InvokeContinuationTyped(data, dataOrigin, histNode);
                caught = InternalEventResult.CombineExceptions(caught, result.Exception);
            }

            return new InternalEventResult<R>(@event.GetEventResult(), caught);
        }


        InternalEventResult IHandlerInvoker<T>.InvokeWithRelatedData(object? data, T inputData, DataOrigin origin, ICreatedEvent? histNode)
            => InvokeWithRelatedData(data, inputData, origin, histNode);

        public InternalEventResult<R> InvokeWithRelatedData(object? data, T inputData, DataOrigin dataOrigin, ICreatedEvent? histNode)
        {
            TypedInvokedEvent2<T, R> @event;
            ExceptionDispatchInfo? caught = null;

            using (var allocated = handler.Manager.Allocator
                .AllocateInOutTyped<TypedInvokedEvent2<T, R>, T, R>(new(dataOrigin, handler.Event, this, data, histNode)))
            {
                try
                {
                    handler.HandlerFunc.Invoke(allocated.Object, Maybe.Some(inputData));
                }
                catch (Exception e)
                {
                    caught = ExceptionDispatchInfo.Capture(e);
                }

                // at this point we don't need to keep it allocated, we can copy out of the heap 
                @event = allocated.Object.Reset();
            }

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                var result = InvokeContinuationRelatedTyped(data, inputData, dataOrigin, histNode);
                caught = InternalEventResult.CombineExceptions(caught, result.Exception);
            }

            return new InternalEventResult<R>(@event.GetEventResult(), caught);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult<R> InvokeContinuationDynamic(dynamic? data, DataOrigin origin, ICreatedEvent? histNode)
        {
            return continuation.InvokeWithData((object?)data, origin, histNode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult<R> InvokeContinuationTyped(in T data, DataOrigin origin, ICreatedEvent? histNode)
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
        internal InternalEventResult<R> InvokeContinuationRelatedTyped(object? data, in T inputData, DataOrigin origin, ICreatedEvent? histNode)
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
