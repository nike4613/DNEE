using DNEE.Internal;
using DNEE.Internal.Resources;
using DNEE.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        // this is the best I can do for a ConcurrentSet
        private readonly ConcurrentDictionary<ITypeConverter, byte> typeConverters;

        /// <summary>
        /// Gets the set of <see cref="ITypeConverter"/>s that currently affect handlers subscribed through this
        /// <see cref="EventSource"/>.
        /// </summary>
        public IEnumerable<ITypeConverter> TypeConverters { get; }

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
        public EventSource(DataOrigin origin) : this(origin, null)
        {
        }


        /// <summary>
        /// Creates a new <see cref="EventSource"/> using the <see cref="DataOrigin"/> <paramref name="origin"/>.
        /// This origin cannot be associated with any other <see cref="EventSource"/>.
        /// </summary>
        /// <param name="origin">The origin to use for this <see cref="EventSource"/>.</param>
        /// <param name="converters">The set of converters to initialize this <see cref="EventSource"/> with.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="origin"/> is already associated with another <see cref="EventSource"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="origin"/> is <see langword="null"/>.</exception>
        public EventSource(DataOrigin origin, IEnumerable<ITypeConverter>? converters)
        {
            if (origin is null)
                throw new ArgumentNullException(nameof(origin));
            if (origin.IsValid)
                throw new ArgumentException(SR.EventSource_OriginAlreadyAttached, nameof(origin));

            typeConverters = converters is null ? new() : new(converters.Select(k => new KeyValuePair<ITypeConverter, byte>(k, 0)));
            TypeConverters = new LazyEnumerable<ITypeConverter>(() => typeConverters.Keys);

            originAssocObj = new();
            Origin = origin;
            origin.SetSource(originAssocObj);

            if (!origin.IsValid)
                throw new ArgumentException(SR.EventSource_OriginAlreadyAttached, nameof(origin));
        }

        public void AddConverter(ITypeConverter converter)
        {
            if (converter is null)
                throw new ArgumentNullException(nameof(converter));

            typeConverters.TryAdd(converter, 0);
        }

        public bool RemoveConverter(ITypeConverter converter)
        {
            if (converter is null)
                throw new ArgumentNullException(nameof(converter));

            return typeConverters.TryRemove(converter, out _);
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
        /// Sets the base of the event identified by <paramref name="derived"/> to the event identified by <paramref name="base"/>.
        /// </summary>
        /// <remarks>
        /// When an event has a base, all of the base's handlers are invoked when the derived event is invoked.
        /// </remarks>
        /// <param name="derived">The event to set the base of.</param>
        /// <param name="base">The event that will be the base.</param>
        /// <exception cref="ArgumentException">Thrown if either <paramref name="derived"/> or <paramref name="base"/> is invalid
        /// -OR- if <paramref name="derived"/> has an origin that does not correspond to this event source.</exception>
        public void SetBase(in EventName derived, in EventName @base)
        {
            if (!derived.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(derived));
            if (!@base.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@base));
            if (Origin != derived.Origin)
                throw new ArgumentException(SR.EventSource_CannotChangeInheritanceOfEvent, nameof(derived));

            EventManager.SetBaseInternal(this, derived, @base);
        }

        /// <summary>
        /// Removes a previously set base for the event identified by <paramref name="derived"/>.
        /// </summary>
        /// <param name="derived">The event to remove the base from.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="derived"/> is invalid
        /// -OR- if <paramref name="derived"/> has an origin that does not correspond to this event source.</exception>
        public void RemoveBase(in EventName derived)
        {
            if (!derived.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(derived));
            if (Origin != derived.Origin)
                throw new ArgumentException(SR.EventSource_CannotChangeInheritanceOfEvent, nameof(derived));

            EventManager.RemoveBaseInternal(this, derived);
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
