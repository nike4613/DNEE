using DNEE.Internal.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE
{
    /// <summary>
    /// An object which manages event subscriptions and dispatching.
    /// </summary>
    /// <remarks>
    /// All subscription methods require a <see cref="DataOriginOwner"/> to allow the event system to keep track of the
    /// data origin for data provided by that handler.
    /// </remarks>
    public sealed class EventManager
    {
        internal Internal.EventManager Internal { get; } = new();

        /// <summary>
        /// Subscribes to the event identified by <paramref name="event"/> with the handler <paramref name="handler"/>
        /// and priority <paramref name="priority"/>.
        /// </summary>
        /// <param name="owner">The <see cref="DataOriginOwner"/> for the subscriber.</param>
        /// <param name="event">The event to subscribe to.</param>
        /// <param name="handler">The handler to subscribe to the event with.</param>
        /// <param name="priority">The priority with which <paramref name="handler"/> should be invoked. Higher is earlier.</param>
        /// <returns>An <see cref="EventHandle"/> representing the subscription.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="event"/> is not valid.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="owner"/> or <paramref name="handler"/> is null.</exception>
        public EventHandle SubscribeTo(DataOriginOwner owner, in EventName @event, DynamicEventHandler handler, HandlerPriority priority)
        {
            if (owner is null)
                throw new ArgumentNullException(nameof(owner));
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return Internal.SubscribeInternal(owner, @event, handler, priority);
        }

        /// <summary>
        /// Subscribes to the event identified by <paramref name="event"/> with the handler <paramref name="handler"/>
        /// and priority <paramref name="priority"/>.
        /// </summary>
        /// <typeparam name="T">The type that the handler expects from its data.</typeparam>
        /// <param name="owner">The <see cref="DataOriginOwner"/> for the subscriber.</param>
        /// <param name="event">The event to subscribe to.</param>
        /// <param name="handler">The handler to subscribe to the event with.</param>
        /// <param name="priority">The priority with which <paramref name="handler"/> should be invoked. Higher is earlier.</param>
        /// <returns>An <see cref="EventHandle"/> representing the subscription.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="event"/> is not valid.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="handler"/> is null.</exception>
        public EventHandle SubscribeTo<T>(DataOriginOwner owner, in EventName @event, NoReturnEventHandler<T> handler, HandlerPriority priority)
        {
            if (owner is null)
                throw new ArgumentNullException(nameof(owner));
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return Internal.SubscribeInternal(owner, @event, handler, priority);
        }

        /// <summary>
        /// Subscribes to the event identified by <paramref name="event"/> with the handler <paramref name="handler"/>
        /// and priority <paramref name="priority"/>.
        /// </summary>
        /// <typeparam name="T">The type that the handler expects from its data.</typeparam>
        /// <typeparam name="TRet">The type that the handler expects to return.</typeparam>
        /// <param name="owner">The <see cref="DataOriginOwner"/> for the subscriber.</param>
        /// <param name="event">The event to subscribe to.</param>
        /// <param name="handler">The handler to subscribe to the event with.</param>
        /// <param name="priority">The priority with which <paramref name="handler"/> should be invoked. Higher is earlier.</param>
        /// <returns>An <see cref="EventHandle"/> representing the subscription.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="event"/> is not valid.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="handler"/> is null.</exception>
        public EventHandle SubscribeTo<T, TRet>(DataOriginOwner owner, in EventName @event, ReturnEventHandler<T, TRet> handler, HandlerPriority priority)
        {
            if (owner is null)
                throw new ArgumentNullException(nameof(owner));
            if (!@event.IsValid)
                throw new ArgumentException(SR.EventNameInvalid, nameof(@event));
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return Internal.SubscribeInternal(owner, @event, handler, priority);
        }

        /// <summary>
        /// Unsubscribes the handler represented by <paramref name="handle"/> from its event.
        /// </summary>
        /// <param name="owner">The <see cref="DataOriginOwner"/> for the subscriber.</param>
        /// <param name="handle">The <see cref="EventHandle"/> representing the handler to unsubscribe.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="handle"/> is not valid 
        /// -OR- if <paramref name="handle"/> was not subscribed with this <see cref="EventSource"/>.</exception>
        /// <exception cref="AggregateException">Thrown if the unsubscription handler(s) from <paramref name="handle"/>
        /// threw. When this is thrown, the event is still unsubscribed.</exception>
        public void Unsubscribe(DataOriginOwner owner, in EventHandle handle)
        {
            if (owner is null)
                throw new ArgumentNullException(nameof(owner));
            if (!handle.IsValid)
                throw new ArgumentException(SR.EventHandleInvalid, nameof(handle));
            if (handle.Origin != owner.Origin)
                throw new ArgumentException(SR.EventSource_HandleNotFromThisSource, nameof(handle));

            Internal.UnsubscribeInternal(handle);
        }
    }
}
