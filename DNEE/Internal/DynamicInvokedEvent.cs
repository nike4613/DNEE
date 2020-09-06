using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    internal sealed class DynamicInvokedEvent : IEvent, IEventWithResult, IDataHistoryNode
    {
        private readonly DynamicInvoker invoker;

        public DynamicInvokedEvent(DataOrigin dataOrigin, in EventName name, DynamicInvoker invoker, dynamic? data, IDataHistoryNode? histNode)
        {
            Data = data;
            DataOrigin = dataOrigin;
            EventName = name;
            this.invoker = invoker;
            nextNode = histNode;
            DataHistory = new DataHistoryEnumerable(this);
        }

        public EventName EventName { get; }

        private Maybe<dynamic?> result = Maybe.None;
        public dynamic? Result { set => result = Maybe.Some((object?)value); }
        public bool DidCallNext { get; private set; } = false;
        public bool AlwaysInvokeNext { get; set; } = true;

        public DataOrigin DataOrigin { get; }

        public DataOrigin Origin => DataOrigin;

        public dynamic? Data { get; }

        private readonly IDataHistoryNode? nextNode;
        IDataHistoryNode? IDataHistoryNode.Next => nextNode;

        public IEnumerable<DataWithOrigin> DataHistory { get; }

        public EventResult Next(dynamic? data)
        {
            if (DidCallNext)
                throw new InvalidOperationException(SR.Handler_NextInvokedOnceOnly);
                
            DidCallNext = true;
            return invoker.InvokeContinuation((object?)data, invoker.Origin, this).Unwrap();
        }

        public EventResult GetEventResult()
            => result.HasValue ? new EventResult((object?)result.Value) : default;
    }
}
