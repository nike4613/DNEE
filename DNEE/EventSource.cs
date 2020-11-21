using DNEE.Internal;
using DNEE.Internal.Resources;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace DNEE
{
    /// <summary>
    /// A source of DNEE events. Also provides a way to subscribe to events.
    /// </summary>
    /// <remarks>
    /// For security, <b>DO NOT ALLOW ACCESS TO YOUR <see cref="EventSource"/>. 
    /// THAT WOULD ALLOW OTHERS TO SEND EVENTS AS YOU.</b>
    /// </remarks>
    public sealed class EventSource
    {
        /// <summary>
        /// The <see cref="DataOrigin"/> associated with this <see cref="EventSource"/>.
        /// </summary>
        public DataOrigin Origin { get; }
        private readonly object originAssocObj;

        /// <summary>
        /// Creates a new <see cref="EventSource"/> with its own <see cref="DataOrigin"/> named <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="DataOrigin"/> to create.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is null or whitespace.</exception>
        public EventSource(string name)
            : this(new DataOrigin(name, null, 1))
        {
        }

        /// <summary>
        /// Creates a new <see cref="EventSource"/> with its own <see cref="DataOrigin"/> named <paramref name="name"/>,
        /// marked as for the member represented by <paramref name="forMember"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="DataOrigin"/> to create.</param>
        /// <param name="forMember">The member that this <see cref="EventSource"/> is for.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is null or whitespace.</exception>
        public EventSource(string name, MemberInfo forMember)
            : this(new DataOrigin(name, forMember, 1))
        {
        }

        /// <summary>
        /// Creates a new <see cref="EventSource"/> using the <see cref="DataOrigin"/> <paramref name="origin"/>.
        /// This origin cannot be associated with any other <see cref="EventSource"/>.
        /// </summary>
        /// <param name="origin">The origin to use for this <see cref="EventSource"/>.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="origin"/> is already associated with another <see cref="EventSource"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="origin"/> is <see langword="null"/>.</exception>
        public EventSource(DataOrigin origin)
        {
            if (origin is null)
                throw new ArgumentNullException(nameof(origin));
            if (origin.IsValid)
                throw new ArgumentException(SR.EventSource_OriginAlreadyAttached, nameof(origin));

            originAssocObj = new();
            Origin = origin;
            origin.SetSource(originAssocObj);

            if (!origin.IsValid)
                throw new ArgumentException(SR.EventSource_OriginAlreadyAttached, nameof(origin));
        }

        /// <summary>
        /// Creates an <see cref="EventName"/> using <paramref name="name"/> in this object's <see cref="Origin"/>.
        /// </summary>
        /// <remarks>
        /// Equivalent to <c><see langword="new"/> <see cref="EventName"/>(source.Origin, <paramref name="name"/>)</c>.
        /// </remarks>
        /// <param name="name">The name of the event.</param>
        /// <returns>An <see cref="EventName"/> represending an event that can be subscribed to and invoked.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null or whitespace.</exception>
        public EventName Event(string name)
            => new EventName(Origin, name);

        /// <summary>
        /// Subscribes to the event identified by <paramref name="event"/> with the handler <paramref name="handler"/>
        /// and priority <paramref name="priority"/>.
        /// </summary>
        /// <param name="event">The event to subscribe to.</param>
        /// <param name="handler">The handler to subscribe to the event with.</param>
        /// <param name="priority">The priority with which <paramref name="handler"/> should be invoked. Higher is earlier.</param>
        /// <returns>An <see cref="EventHandle"/> representing the subscription.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="event"/> is not valid.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="handler"/> is null.</exception>
        public EventHandle SubscribeTo(in EventName @event, DynamicEventHandler handler, HandlerPriority priority)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return EventManager.SubscribeInternal(this, @event, handler, priority);
        }

        /// <summary>
        /// Subscribes to the event identified by <paramref name="event"/> with the handler <paramref name="handler"/>
        /// and priority <paramref name="priority"/>.
        /// </summary>
        /// <typeparam name="T">The type that the handler expects from its data.</typeparam>
        /// <param name="event">The event to subscribe to.</param>
        /// <param name="handler">The handler to subscribe to the event with.</param>
        /// <param name="priority">The priority with which <paramref name="handler"/> should be invoked. Higher is earlier.</param>
        /// <returns>An <see cref="EventHandle"/> representing the subscription.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="event"/> is not valid.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="handler"/> is null.</exception>
        public EventHandle SubscribeTo<T>(in EventName @event, NoReturnEventHandler<T> handler, HandlerPriority priority)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return EventManager.SubscribeInternal(this, @event, handler, priority);
        }

        /// <summary>
        /// Subscribes to the event identified by <paramref name="event"/> with the handler <paramref name="handler"/>
        /// and priority <paramref name="priority"/>.
        /// </summary>
        /// <typeparam name="T">The type that the handler expects from its data.</typeparam>
        /// <typeparam name="TRet">The type that the handler expects to return.</typeparam>
        /// <param name="event">The event to subscribe to.</param>
        /// <param name="handler">The handler to subscribe to the event with.</param>
        /// <param name="priority">The priority with which <paramref name="handler"/> should be invoked. Higher is earlier.</param>
        /// <returns>An <see cref="EventHandle"/> representing the subscription.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="event"/> is not valid.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="handler"/> is null.</exception>
        public EventHandle SubscribeTo<T, TRet>(in EventName @event, ReturnEventHandler<T, TRet> handler, HandlerPriority priority)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return EventManager.SubscribeInternal(this, @event, handler, priority);
        }

        /// <summary>
        /// Unsubscribes the handler represented by <paramref name="handle"/> from its event.
        /// </summary>
        /// <param name="handle">The <see cref="EventHandle"/> representing the handler to unsubscribe.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="handle"/> is not valid 
        /// -OR- if <paramref name="handle"/> was not subscribed with this <see cref="EventSource"/>.</exception>
        /// <exception cref="AggregateException">Thrown if the unsubscription handler(s) from <paramref name="handle"/>
        /// threw. When this is thrown, the event is still unsubscribed.</exception>
        public void Unsubscribe(in EventHandle handle)
        {
            if (!handle.IsValid)
                throw new ArgumentException(SR.EventHandleInvalid, nameof(handle));
            if (handle.Origin != Origin)
                throw new ArgumentException(SR.EventSource_HandleNotFromThisSource, nameof(handle));

            EventManager.UnsubscribeInternal(handle);
        }

        /// <summary>
        /// Invokes the handlers for the event identified by <paramref name="event"/> with <paramref name="data"/> as their argument.
        /// </summary>
        /// <param name="event">The event to invoke.</param>
        /// <param name="data">The data to pass to the handlers.</param>
        /// <returns>An <see cref="EventResult"/> representing the result of the invocations.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="event"/> is not valid.</exception>
        /// <exception cref="HandlerInvocationException">Thrown if the event handlers throw. <see cref="Exception.InnerException"/> 
        /// contains the thrown exception.</exception>
        public EventResult SendEventDynamic(in EventName @event, dynamic? data)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));

            try
            {
                return EventManager.DynamicSendInternal(this, @event, (object?)data, null).Unwrap();
            }
            catch (Exception e)
            {
                throw new HandlerInvocationException(string.Format(SR.Culture, SR.ErrorInvokingEvents, @event), e);
            }
        }

        /// <summary>
        /// Invokes the handlers for the event identified by <paramref name="event"/> with <paramref name="data"/> as their argument.
        /// </summary>
        /// <typeparam name="T">The type of the data to pass to the handlers.</typeparam>
        /// <param name="event">The event to invoke.</param>
        /// <param name="data">The data to pass to the handlers.</param>
        /// <returns>An <see cref="EventResult"/> representing the result of the invocations.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="event"/> is not valid.</exception>
        /// <exception cref="HandlerInvocationException">Thrown if the event handlers throw. <see cref="Exception.InnerException"/> contains the thrown exception.</exception>
        public EventResult SendEvent<T>(in EventName @event, in T data)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));

            try
            {
                return EventManager.TypedSendInternal(this, @event, data, null).Unwrap();
            }
            catch (Exception e)
            {
                throw new HandlerInvocationException(string.Format(SR.Culture, SR.ErrorInvokingEvents, @event), e);
            }
        }

        /// <summary>
        /// Invokes the handlers for the event identified by <paramref name="event"/> with <paramref name="data"/> as their argument.
        /// </summary>
        /// <typeparam name="T">The type of the data to pass to the handlers.</typeparam>
        /// <typeparam name="TRet">The type that the event is expected to return.</typeparam>
        /// <param name="event">The event to invoke.</param>
        /// <param name="data">The data to pass to the handlers.</param>
        /// <returns>An <see cref="EventResult{T}"/> representing the result of the invocations.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="event"/> is not valid.</exception>
        /// <exception cref="HandlerInvocationException">Thrown if the event handlers throw. <see cref="Exception.InnerException"/> contains the thrown exception.</exception>
        public EventResult<TRet> SendEvent<T, TRet>(in EventName @event, in T data)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));

            try
            {
                return EventManager.TypedSendInternal<T, TRet>(this, @event, data, null).Unwrap();
            }
            catch (Exception e)
            {
                throw new HandlerInvocationException(string.Format(SR.Culture, SR.ErrorInvokingEvents, @event), e);
            }
        }

        private static IDataHistoryNode? HistoryNodeForEvent(IEvent parent)
        {
            IEvent? lastParent = null;
            while (parent is IEventWrapper wrapper && !ReferenceEquals(lastParent, parent))
            {
                lastParent = parent;
                parent = wrapper.BaseEvent;
            }

            return parent as IDataHistoryNode;
        }

        /// <summary>
        /// Invokes the handlers for the event identified by <paramref name="event"/> with <paramref name="data"/> as their argument.
        /// </summary>
        /// <remarks>
        /// The data history of <paramref name="parent"/> will be included in the history of the new event invocation.
        /// </remarks>
        /// <param name="event">The event to invoke.</param>
        /// <param name="data">The data to pass to the handlers.</param>
        /// <param name="parent">The event which is the parent of the new invocation.</param>
        /// <returns>An <see cref="EventResult"/> representing the result of the invocations.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="event"/> is not valid.</exception>
        /// <exception cref="HandlerInvocationException">Thrown if the event handlers throw. <see cref="Exception.InnerException"/> 
        /// contains the thrown exception.</exception>
        public EventResult SendEventDynamic(in EventName @event, dynamic? data, IEvent parent)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));

            if (parent is null)
                throw new ArgumentNullException(nameof(parent));

            var node = HistoryNodeForEvent(parent);

            try
            {
                return EventManager.DynamicSendInternal(this, @event, (object?)data, node).Unwrap();
            }
            catch (Exception e)
            {
                throw new HandlerInvocationException(string.Format(SR.Culture, SR.ErrorInvokingEvents, @event), e);
            }
        }

        /// <summary>
        /// Invokes the handlers for the event identified by <paramref name="event"/> with <paramref name="data"/> as their argument.
        /// </summary>
        /// <remarks>
        /// The data history of <paramref name="parent"/> will be included in the history of the new event invocation.
        /// </remarks>
        /// <typeparam name="T">The type of the data to pass to the handlers.</typeparam>
        /// <param name="event">The event to invoke.</param>
        /// <param name="data">The data to pass to the handlers.</param>
        /// <param name="parent">The event which is the parent of the new invocation.</param>
        /// <returns>An <see cref="EventResult"/> representing the result of the invocations.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="event"/> is not valid.</exception>
        /// <exception cref="HandlerInvocationException">Thrown if the event handlers throw. <see cref="Exception.InnerException"/> contains the thrown exception.</exception>
        public EventResult SendEvent<T>(in EventName @event, in T data, IEvent parent)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));

            if (parent is null)
                throw new ArgumentNullException(nameof(parent));

            var node = HistoryNodeForEvent(parent);

            try
            {
                return EventManager.TypedSendInternal(this, @event, data, node).Unwrap();
            }
            catch (Exception e)
            {
                throw new HandlerInvocationException(string.Format(SR.Culture, SR.ErrorInvokingEvents, @event), e);
            }
        }

        /// <summary>
        /// Invokes the handlers for the event identified by <paramref name="event"/> with <paramref name="data"/> as their argument.
        /// </summary>
        /// <remarks>
        /// The data history of <paramref name="parent"/> will be included in the history of the new event invocation.
        /// </remarks>
        /// <typeparam name="T">The type of the data to pass to the handlers.</typeparam>
        /// <typeparam name="TRet">The type that the event is expected to return.</typeparam>
        /// <param name="event">The event to invoke.</param>
        /// <param name="data">The data to pass to the handlers.</param>
        /// <param name="parent">The event which is the parent of the new invocation.</param>
        /// <returns>An <see cref="EventResult{T}"/> representing the result of the invocations.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="event"/> is not valid.</exception>
        /// <exception cref="HandlerInvocationException">Thrown if the event handlers throw. <see cref="Exception.InnerException"/> contains the thrown exception.</exception>
        public EventResult<TRet> SendEvent<T, TRet>(in EventName @event, in T data, IEvent parent)
        {
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));

            if (parent is null)
                throw new ArgumentNullException(nameof(parent));

            var node = HistoryNodeForEvent(parent);

            try
            {
                return EventManager.TypedSendInternal<T, TRet>(this, @event, data, node).Unwrap();
            }
            catch (Exception e)
            {
                throw new HandlerInvocationException(string.Format(SR.Culture, SR.ErrorInvokingEvents, @event), e);
            }
        }
    }
}
