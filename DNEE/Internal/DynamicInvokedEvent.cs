using DNEE.Internal.Resources;
using DNEE.Tuning;
using DNEE.Utility;
using System;
using System.Collections.Generic;

namespace DNEE.Internal
{
    internal struct DynamicInvokedEvent : IEvent, IInternalEventImpl<DynamicInvokedEvent>
    {
        private readonly DynamicInvoker invoker;

        public DynamicInvokedEvent(DataOrigin dataOrigin, in EventName name, DynamicInvoker invoker, dynamic? data, ICreatedEvent? histNode)
        {
            Data = data;
            DataOrigin = dataOrigin;
            EventName = name;
            this.invoker = invoker;
            lastEvent = histNode;
            lazyDataHistory = null;
            Holder = null;
            DidCallNext = false;
        }

        public ICreatedEvent<DynamicInvokedEvent>? Holder { get; set; }
        object IInternalEvent<DynamicInvokedEvent>.InterfaceContract => InterfaceContract.Instance;
        void IInternalEvent<DynamicInvokedEvent>.SetHolder(ICreatedEvent<DynamicInvokedEvent> created) => Holder = created;

        private sealed class InterfaceContract : EventInterfaceContract<DynamicInvokedEvent>
        {
            public static readonly InterfaceContract Instance = new();

            public override ICreatedEvent? GetLastEventCore(ref DynamicInvokedEvent @event) => @event.lastEvent;
        }

        private readonly ICreatedEvent? lastEvent;

        public EventName EventName { get; }

        private Maybe<dynamic?> result = Maybe.None;
        public dynamic? Result 
        {
            get => result.ValueOr(default);
            set => result = Maybe.Some((object?)value);
        }
        public bool DidCallNext { get; private set; }
        public bool AlwaysInvokeNext { get; set; } = true;

        public DataOrigin DataOrigin { get; }

        public DataOrigin Origin => DataOrigin;

        public dynamic? Data { get; }

        // TODO: this should probably be cleared during reset, but it won't live very long anyway, and *probably* wasn't used
        private DataHistoryEnumerable? lazyDataHistory;
        public IEnumerable<DataWithOrigin> DataHistory => lazyDataHistory ??= new DataHistoryEnumerable(Holder ?? throw new InvalidOperationException());

        public EventResult Next()
        {
            if (DidCallNext)
                throw new InvalidOperationException(SR.Handler_NextInvokedOnceOnly);

            DidCallNext = true;
            return invoker.InvokeContinuation((object?)Data, DataOrigin, Holder).Unwrap();
        }

        public EventResult Next(dynamic? data)
        {
            if (DidCallNext)
                throw new InvalidOperationException(SR.Handler_NextInvokedOnceOnly);
                
            DidCallNext = true;
            return invoker.InvokeContinuation((object?)data, invoker.Origin, Holder).Unwrap();
        }

        public EventResult GetEventResult()
            => result.HasValue ? new EventResult((object?)result.Value) : default;
    }
}
