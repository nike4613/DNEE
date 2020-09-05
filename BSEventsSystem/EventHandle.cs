using DNEE.Internal;
using System;

namespace DNEE
{
    public struct EventHandle : IDisposable
    {
        public bool IsValid => Cell != null && Handler != null;

        internal readonly EventManager.HandlerSetCell Cell;
        internal readonly IHandler Handler;

        internal EventHandle(EventManager.HandlerSetCell cell, IHandler handler)
        {
            Cell = cell;
            Handler = handler;
        }

        public void Dispose()
        {
            EventManager.UnregisterHandler(this);
        }
    }
}