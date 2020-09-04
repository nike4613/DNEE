using BSEventsSystem.Internal;

namespace BSEventsSystem
{
    public struct EventHandle
    {
        public bool IsValid => Cell != null && Handler != null;

        internal readonly EventManager.HandlerSetCell Cell;
        internal readonly IHandler Handler;

        internal EventHandle(EventManager.HandlerSetCell cell, IHandler handler)
        {
            Cell = cell;
            Handler = handler;
        }
    }
}