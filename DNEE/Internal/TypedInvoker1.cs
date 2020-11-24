using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

            // TODO: should this actually call some other invoke that correctly passes on the original data when its continued automatically?
            var converter = handler.Converters.FirstOrDefault(c => c.CanConvertTo<T, object?>((object?)data));
            if (converter != null)
                return InvokeWithData(converter.ConvertTo<T, object?>((object?)data), dataOrigin, histNode);

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

        [SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification = "Exceptions are caught and automatically propagated using a custom system that minimizes" +
                            "our stack in user stack traces.")]
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

        [SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification = "Exceptions are caught and automatically propagated using a custom system that minimizes" +
                            "our stack in user stack traces.")]
        public InternalEventResult InvokeWithUsableData(IUsableAs<T> data, DataOrigin dataOrigin, IDataHistoryNode? histNode)
        {
            var @event = new TypedInvokedEvent1<T>(dataOrigin, handler.Event, this, data, histNode);

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
        internal InternalEventResult InvokeContinuationUsableTyped(IUsableAs<T> data, DataOrigin origin, IDataHistoryNode? histNode)
        {
            if (continuation is IHandlerInvoker<T> typed)
                return typed.InvokeWithUsableData(data, origin, histNode);
            else
                return continuation.InvokeWithData(data, origin, histNode);
        }
    }
}
