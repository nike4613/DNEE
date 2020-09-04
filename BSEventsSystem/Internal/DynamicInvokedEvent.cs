using BSEventsSystem.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSEventsSystem.Internal
{
    internal sealed class DynamicInvokedEvent : IEvent, IEventWithResult
    {
        public DynamicInvokedEvent(EventName name)
        {
            EventName = name;
        }

        public EventName EventName { get; }

        private Maybe<dynamic?> result = Maybe.None;
        public dynamic? Result { set => result = Maybe.Some(value); }
        public bool DidCallNext { get; private set; } = false;
        public bool AlwaysInvokeNext { get; set; } = true;

        public EventResult Next(dynamic data)
        {
            DidCallNext = true;

            throw new NotImplementedException();
        }

        public EventResult GetEventResult()
            => result.HasValue ? new EventResult(result.Value) : default;
    }
}
