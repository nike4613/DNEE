using System;
using System.Collections.Generic;
using System.Text;

namespace BSEventsSystem
{
    public static partial class EventManager
    {


        #region Register/Unregister
        private static EventHandle RegisterInternal(in EventName @event, DynamicEventHandler handler, HandlerPriority priority)
        {
            throw new NotImplementedException();
        }

        private static EventHandle RegisterInternal<T>(in EventName @event, NoReturnEventHandler<T> handler, HandlerPriority priority)
        {
            throw new NotImplementedException();
        }

        private static EventHandle RegisterInternal<T, R>(in EventName @event, ReturnEventHandler<T, R> handler, HandlerPriority priority)
        {
            throw new NotImplementedException();
        }

        private static void UnregisterInternal(in EventHandle handle)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Send
        private static EventResult DynamicSendInternal(EventName @event, dynamic? data)
        {
            throw new NotImplementedException();
        }

        private static EventResult TypedSendInternal<T>(EventName @event, in T data)
        {
            throw new NotImplementedException();
        }
        
        private static EventResult<R> TypedSendInternal<T, R>(EventName @event, in T data)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
