using DNEE.Internal;
using DNEE.Internal.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DNEE
{
    /// <summary>
    /// A handle representing an event handler subscribed to an event.
    /// </summary>
    public struct EventHandle : IDisposable, IEquatable<EventHandle>
    {
        /// <summary>
        /// Gets whether or not this handle is valid.
        /// </summary>
        public bool IsValid => Manager != null && Handler != null && Origin != null;

        internal readonly Internal.EventManager Manager;
        internal readonly EventName Event;
        internal readonly DataOrigin Origin;
        internal readonly IHandler Handler;

        internal EventHandle(Internal.EventManager manager, EventName name, IHandler handler, DataOrigin origin)
        {
            Manager = manager;
            Event = name;
            Handler = handler;
            Origin = origin;
            UnsubEvent = () => { };
        }

        private event Action UnsubEvent;

        internal void InvokeUnsubEvents()
        {
            var exceptions = new List<Exception>();
            foreach (var h in UnsubEvent.GetInvocationList().Cast<Action>())
            {
                try
                {
                    h();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Count != 0)
            {
                throw new AggregateException(
                    string.Format(SR.Culture, SR.EventHandle_UnsubHandlersThrew, Handler.Event),
                    exceptions
                );
            }
        }

        /// <summary>
        /// Registers an event handler to be invoked whenever this <see cref="EventHandle"/> is unsubscribed (if it is).
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to call on unsubscription.</param>
        /// <returns><see langword="this"/>, for easy chaining.</returns>
        public EventHandle OnUnsubscribe(Action action)
        {
            UnsubEvent -= action;
            UnsubEvent += action;
            return this;
        }

        /// <summary>
        /// Unsubscribes the handler represented by this handle from its associated event.
        /// </summary>
        public void Dispose()
        {
            if (IsValid) Manager.UnsubscribeInternal(this);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => obj is EventHandle eh && Equals(eh);

        /// <inheritdoc/>
        public bool Equals(EventHandle other)
            => Origin == other.Origin && Handler == other.Handler && UnsubEvent == other.UnsubEvent;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -1433727288;
            hashCode = hashCode * -1521134295 + IsValid.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<DataOrigin>.Default.GetHashCode(Origin);
            hashCode = hashCode * -1521134295 + EqualityComparer<IHandler>.Default.GetHashCode(Handler);
            return hashCode;
        }

        /// <summary>
        /// Compares two <see cref="EventHandle"/>s for equality.
        /// </summary>
        /// <param name="left">The first handle to compare.</param>
        /// <param name="right">The second handle to compare.</param>
        /// <returns><see langword="true"/> if they are equal, <see langword="false"/> otherwise.</returns>
        public static bool operator ==(EventHandle left, EventHandle right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="EventHandle"/>s for inequality.
        /// </summary>
        /// <param name="left">The first handle to compare.</param>
        /// <param name="right">The second handle to compare.</param>
        /// <returns><see langword="true"/> if they are not equal, <see langword="false"/> otherwise.</returns>
        public static bool operator !=(EventHandle left, EventHandle right) => !(left == right);
    }
}