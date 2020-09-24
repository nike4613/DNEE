using DNEE.Internal;
using DNEE.Internal.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DNEE
{
    /// <summary>
    /// A handle representing an event handler subscribed to an event.
    /// </summary>
    public struct EventHandle : IDisposable
    {
        /// <summary>
        /// Gets whether or not this handle is valid.
        /// </summary>
        public bool IsValid => Cell != null && Handler != null && Origin != null;

        internal readonly DataOrigin Origin;
        internal readonly EventManager.HandlerSetCell Cell;
        internal readonly IHandler Handler;

        internal EventHandle(EventManager.HandlerSetCell cell, IHandler handler, DataOrigin origin)
        {
            Cell = cell;
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
                    string.Format(SR.EventHandle_UnsubHandlersThrew, Handler.Event),
                    exceptions
                );
            }
        }

        /// <summary>
        /// Registers an event handler to be invoked whenever this <see cref="EventHandle"/> is unsubscribed (if it is).
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to call on unsubscription</param>
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
            EventManager.UnsubscribeInternal(this);
        }
    }
}