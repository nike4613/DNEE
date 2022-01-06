using DNEE.Internal;
using DNEE.Internal.Resources;
using DNEE.Tuning;
using System;

namespace DNEE
{
    /// <summary>
    /// A source of DNEE events. This is attached to a specific <see cref="EventManager"/>
    /// </summary>
    /// <remarks>
    /// For security, <b>DO NOT ALLOW ACCESS TO YOUR <see cref="EventSource"/>. 
    /// THAT WOULD ALLOW OTHERS TO SEND EVENTS AS YOU.</b>
    /// </remarks>
    public sealed class EventSource
    {
        /// <summary>
        /// Gets the <see cref="DataOriginOwner"/> associated with this <see cref="EventSource"/>.
        /// </summary>
        public DataOriginOwner OriginOwner { get; }
        /// <summary>
        /// Gets the origin associated with this <see cref="EventSource"/>.
        /// </summary>
        public DataOrigin Origin => OriginOwner.Origin;
        /// <summary>
        /// Gets the <see cref="EventManager"/> that this <see cref="EventSource"/> targets.
        /// </summary>
        public EventManager Manager { get; }

        /// <summary>
        /// Creates an <see cref="EventSource"/> associated with the provided <see cref="EventManager"/>
        /// and <see cref="DataOrigin"/>.
        /// </summary>
        /// <param name="manager">The event manager to use.</param>
        /// <param name="originOwner">The origin to use.</param>
        /// <exception cref="ArgumentNullException">Thrown if either parameter is null.</exception>
        public EventSource(EventManager manager, DataOriginOwner originOwner)
        {
            if (manager is null)
                throw new ArgumentNullException(nameof(manager));
            if (originOwner is null)
                throw new ArgumentNullException(nameof(manager));

            Manager = manager;
            OriginOwner = originOwner;
        }

        /// <summary>
        /// Creates an <see cref="EventName"/> using <paramref name="name"/> in this object's <see cref="Origin"/>.
        /// </summary>
        /// <remarks>
        /// Equivalent to <c><see langword="new"/> <see cref="EventName"/>(<see cref="Origin"/>, <paramref name="name"/>)</c>.
        /// </remarks>
        /// <param name="name">The name of the event.</param>
        /// <returns>An <see cref="EventName"/> represending an event that can be subscribed to and invoked.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null or whitespace.</exception>
        public EventName Event(string name) => new(Origin, name);

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

            Manager.Internal.SetBaseInternal(this, derived, @base);
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

            Manager.Internal.RemoveBaseInternal(this, derived);
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
                return Manager.Internal.DynamicSendInternal(this, @event, (object?)data, null).Unwrap();
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
                return Manager.Internal.TypedSendInternal(this, @event, data, null).Unwrap();
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
                return Manager.Internal.TypedSendInternal<T, TRet>(this, @event, data, null).Unwrap();
            }
            catch (Exception e)
            {
                throw new HandlerInvocationException(string.Format(SR.Culture, SR.ErrorInvokingEvents, @event), e);
            }
        }

        private static ICreatedEvent? GetCreatedEvent(IEvent parent)
        {
            IEvent? lastParent = null;
            while (parent is IEventWrapper wrapper && !ReferenceEquals(lastParent, parent))
            {
                lastParent = parent;
                parent = wrapper.BaseEvent;
            }

            return parent as ICreatedEvent;
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

            var node = GetCreatedEvent(parent);

            try
            {
                return Manager.Internal.DynamicSendInternal(this, @event, (object?)data, node).Unwrap();
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

            var node = GetCreatedEvent(parent);

            try
            {
                return Manager.Internal.TypedSendInternal(this, @event, data, node).Unwrap();
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

            var node = GetCreatedEvent(parent);

            try
            {
                return Manager.Internal.TypedSendInternal<T, TRet>(this, @event, data, node).Unwrap();
            }
            catch (Exception e)
            {
                throw new HandlerInvocationException(string.Format(SR.Culture, SR.ErrorInvokingEvents, @event), e);
            }
        }
    }
}
