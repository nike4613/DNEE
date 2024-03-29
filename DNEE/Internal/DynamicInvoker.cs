﻿using DNEE.Tuning;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace DNEE.Internal
{
    internal class DynamicInvoker : IHandlerInvoker
    {
        private readonly DynamicHandler handler;
        private readonly IHandlerInvoker continuation;

        public DataOrigin Origin => handler.Origin;

        public DynamicInvoker(DynamicHandler handler, IHandlerInvoker continueWith)
        {
            this.handler = handler;
            continuation = continueWith;
        }

        public InternalEventResult InvokeWithData(dynamic? data, DataOrigin dataOrigin, ICreatedEvent? lastNode)
        {
            DynamicInvokedEvent @event;
            ExceptionDispatchInfo? caught = null;

            using (var allocated = handler.Manager.Allocator
                .AllocateTypeless<DynamicInvokedEvent>(new(dataOrigin, handler.Event, this, (object?)data, lastNode)))
            {
                try
                {
                    handler.HandlerFunc.Invoke(allocated.Object, (object?)data);
                }
                catch (Exception e)
                {
                    caught = ExceptionDispatchInfo.Capture(e);
                }

                @event = allocated.Object.Reset();
            }

            if (@event.AlwaysInvokeNext && !@event.DidCallNext)
            {
                var result = InvokeContinuation((object?)data, dataOrigin, lastNode);
                caught = InternalEventResult.CombineExceptions(caught, result.Exception);
            }

            return new InternalEventResult(@event.GetEventResult(), caught);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InternalEventResult InvokeContinuation(dynamic? data, DataOrigin origin, ICreatedEvent? lastNode)
        {
            return continuation.InvokeWithData((object?)data, origin, lastNode);
        }
    }
}
