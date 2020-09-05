﻿using DNEE.Internal;
using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE
{
    public static partial class EventManager
    {
        public delegate void DynamicEventHandler(IEvent @event, dynamic? data);
        public delegate void NoReturnEventHandler<T>(IEvent<T> @event, Maybe<T> data);
        public delegate void ReturnEventHandler<T, R>(IEvent<T, R> @event, Maybe<T> data);

        public static EventHandle SubscribeTo(in EventName @event, DynamicEventHandler handler, HandlerPriority priority)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return SubscribeInternal(@event, handler, priority);
        }

        public static EventHandle SubscribeTo<T>(in EventName @event, NoReturnEventHandler<T> handler, HandlerPriority priority)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return SubscribeInternal(@event, handler, priority);
        }

        public static EventHandle SubscribeTo<T, R>(in EventName @event, ReturnEventHandler<T, R> handler, HandlerPriority priority)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return SubscribeInternal(@event, handler, priority);
        }

        public static void Unsubscribe(in EventHandle handle)
        {
            if (!handle.IsValid)
                throw new ArgumentException(SR.EventHandleInvalid, nameof(handle));

            UnsubscribeInternal(handle);
        }

        public static EventResult SendEventDynamic(in EventName @event, dynamic? data)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));

            try
            {
                return DynamicSendInternal(@event, (object?)data);
            }
            catch (Exception e)
            {
                throw new HandlerInvocationException(string.Format(SR.ErrorInvokingEvents, @event), e);
            }
        }

        public static EventResult SendEvent<T>(in EventName @event, in T data)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));

            try
            {
                return TypedSendInternal(@event, data);
            }
            catch (Exception e)
            {
                throw new HandlerInvocationException(string.Format(SR.ErrorInvokingEvents, @event), e);
            }
        }

        public static EventResult<R> SendEvent<T, R>(in EventName @event, in T data)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));

            try
            {
                return TypedSendInternal<T, R>(@event, data);
            }
            catch (Exception e)
            {
                throw new HandlerInvocationException(string.Format(SR.ErrorInvokingEvents, @event), e);
            }
        }
    }
}
