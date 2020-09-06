using DNEE.Internal;
using DNEE.Internal.Resources;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace DNEE
{
    public sealed class EventSource
    {
        public DataOrigin Origin { get; }

        public EventSource(string name)
        {
            Origin = new DataOrigin(name, null, 1) { Source = this };
        }

        public EventSource(string name, MemberInfo forMember)
        {
            Origin = new DataOrigin(name, forMember, 1) { Source = this };
        }

        public EventSource(DataOrigin origin)
        {
            if (origin.Source != null)
                throw new ArgumentException(SR.EventSource_OriginAlreadyAttached, nameof(origin));

            origin.Source = this;
            Origin = origin;
        }

        public EventName Event(string name)
            => new EventName(Origin, name);


        public EventHandle SubscribeTo(in EventName @event, DynamicEventHandler handler, HandlerPriority priority)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return EventManager.SubscribeInternal(this, @event, handler, priority);
        }

        public EventHandle SubscribeTo<T>(in EventName @event, NoReturnEventHandler<T> handler, HandlerPriority priority)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return EventManager.SubscribeInternal(this, @event, handler, priority);
        }

        public EventHandle SubscribeTo<T, R>(in EventName @event, ReturnEventHandler<T, R> handler, HandlerPriority priority)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return EventManager.SubscribeInternal(this, @event, handler, priority);
        }

        public void Unsubscribe(in EventHandle handle)
        {
            if (!handle.IsValid)
                throw new ArgumentException(SR.EventHandleInvalid, nameof(handle));
            if (handle.Source != this)
                throw new ArgumentException(SR.EventSource_HandleNotFromThisSource, nameof(handle));

            EventManager.UnsubscribeInternal(handle);
        }

        public EventResult SendEventDynamic(in EventName @event, dynamic? data)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));

            try
            {
                return EventManager.DynamicSendInternal(this, @event, (object?)data).Unwrap();
            }
            catch (Exception e)
            {
                throw new HandlerInvocationException(string.Format(SR.ErrorInvokingEvents, @event), e);
            }
        }

        public EventResult SendEvent<T>(in EventName @event, in T data)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));

            try
            {
                return EventManager.TypedSendInternal(this, @event, data).Unwrap();
            }
            catch (Exception e)
            {
                throw new HandlerInvocationException(string.Format(SR.ErrorInvokingEvents, @event), e);
            }
        }

        public EventResult<R> SendEvent<T, R>(in EventName @event, in T data)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));

            try
            {
                return EventManager.TypedSendInternal<T, R>(this, @event, data).Unwrap();
            }
            catch (Exception e)
            {
                throw new HandlerInvocationException(string.Format(SR.ErrorInvokingEvents, @event), e);
            }
        }
    }
}
