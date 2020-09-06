using DNEE.Internal;
using System;

namespace DNEE
{
    public struct EventHandle : IDisposable
    {
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

        public void Dispose()
        {
            EventManager.UnsubscribeInternal(this);
        }
    }
}