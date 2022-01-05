using DNEE.Tuning;
using DNEE.Utility;
using System;
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

        public InternalEventResult InvokeWithData(dynamic? data, DataOrigin dataOrigin, ICreatedEvent? histNode)
        {
            var obj = (object?)data;
            if (obj is T tval)
                return InvokeWithData(tval, dataOrigin, histNode);
            if (Helpers.TryUseAs<T>((object?)data, out var asType))
                return InvokeWithRelatedData((object?)data, asType, dataOrigin, histNode);

            TypedInvokedEvent1<T> @event;
            ExceptionDispatchInfo? caught = null;
            using (var allocated = handler.Manager.Allocator
                .AllocateInTyped<TypedInvokedEvent1<T>, T>(new(dataOrigin, handler.Event, this, obj, histNode)))
            {
                try
                {
                    handler.HandlerFunc.Invoke(allocated.Object, Maybe.None);
                }
                catch (Exception e)
                {
                    caught = ExceptionDispatchInfo.Capture(e);
                }

                // at this point we don't need to keep it allocated, we can move it out of the heap
                @event = allocated.Object.Reset();
            }

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                var result = InvokeContinuationDynamic((object?)data, dataOrigin, histNode);
                caught = InternalEventResult.CombineExceptions(caught, result.Exception);
            }

            return new InternalEventResult(@event.GetEventResult(), caught);
        }

        public InternalEventResult InvokeWithData(T data, DataOrigin dataOrigin, ICreatedEvent? histNode)
        {
            TypedInvokedEvent1<T> @event;
            ExceptionDispatchInfo? caught = null;

            using (var allocated = handler.Manager.Allocator
                .AllocateInTyped<TypedInvokedEvent1<T>, T>(new (dataOrigin, handler.Event, this, data, histNode)))
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

            return new InternalEventResult(@event.GetEventResult(), caught);
        }

        public InternalEventResult InvokeWithRelatedData(object? data, T inputData, DataOrigin dataOrigin, ICreatedEvent? histNode)
        {
            TypedInvokedEvent1<T> @event;
            ExceptionDispatchInfo? caught = null;

            using (var allocated = handler.Manager.Allocator
                .AllocateInTyped<TypedInvokedEvent1<T>, T>(new(dataOrigin, handler.Event, this, data, histNode)))
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

            return new InternalEventResult(@event.GetEventResult(), caught);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult InvokeContinuationDynamic(dynamic? data, DataOrigin origin, ICreatedEvent? histNode)
        {
            return continuation.InvokeWithData((object?)data, origin, histNode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult InvokeContinuationTyped(in T data, DataOrigin origin, ICreatedEvent? histNode)
        {
            if (continuation is IHandlerInvoker<T> typed)
                return typed.InvokeWithData(data, origin, histNode);
            else
                return continuation.InvokeWithData(data, origin, histNode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult InvokeContinuationRelatedTyped(object? data, in T inputData, DataOrigin origin, ICreatedEvent? histNode)
        {
            if (continuation is IHandlerInvoker<T> typed)
                return typed.InvokeWithRelatedData(data, inputData, origin, histNode);
            else
                return continuation.InvokeWithData(data, origin, histNode);
        }
    }
}
