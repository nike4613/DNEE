using BSEventsSystem.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSEventsSystem.Internal
{
    internal sealed class DynamicInvokedEvent : IEvent, IEventWithResult
    {
        private readonly DynamicInvoker invoker;

        public DynamicInvokedEvent(in EventName name, DynamicInvoker invoker)
        {
            EventName = name;
            this.invoker = invoker;
        }

        public EventName EventName { get; }

        private Maybe<dynamic?> result = Maybe.None;
        public dynamic? Result { set => result = Maybe.Some((object?)value); }
        public bool DidCallNext { get; private set; } = false;
        public bool AlwaysInvokeNext { get; set; } = true;

        private EventResult? nextResult = null;
        public EventResult Next(dynamic? data)
        {
            if (nextResult != null)
                return nextResult.Value;

            DidCallNext = true;

            nextResult = invoker.InvokeContinuation((object?)data);
            return nextResult.Value;
        }

        public EventResult GetEventResult()
            => result.HasValue ? new EventResult((object?)result.Value) : default;
    }
}
