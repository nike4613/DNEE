using DNEE.Internal;
using System;

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
        public bool IsValid => Cell != null && Handler != null && Source != null;

        internal readonly EventSource Source;
        internal readonly EventManager.HandlerSetCell Cell;
        internal readonly IHandler Handler;

        internal EventHandle(EventManager.HandlerSetCell cell, IHandler handler, EventSource source)
        {
            Cell = cell;
            Handler = handler;
            Source = source;
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