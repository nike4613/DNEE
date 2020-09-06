using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    internal sealed class TypedInvokedEvent1<T> : IEvent<T>, IEventWithResult
    {

        private readonly TypedInvoker1<T> invoker;

        public TypedInvokedEvent1(DataOrigin dataOrigin, in EventName name, TypedInvoker1<T> invoker, dynamic? data)
        {
            DataOrigin = dataOrigin;
            EventName = name;
            this.invoker = invoker;
            DynamicData = data;
        }

        public EventName EventName { get; }

        private Maybe<dynamic?> result = Maybe.None;
        public dynamic? Result { set => result = Maybe.Some((object?)value); }
        public bool DidCallNext { get; private set; } = false;
        public bool AlwaysInvokeNext { get; set; } = true;
        public DataOrigin DataOrigin { get; }

        public dynamic? DynamicData { get; }

        public EventResult Next(dynamic? data)
        {
            if (DidCallNext)
                throw new InvalidOperationException(SR.Handler_NextInvokedOnceOnly);

            DidCallNext = true;
            return invoker.InvokeContinuationDynamic((object?)data, invoker.Origin).Unwrap();
        }

        public EventResult Next(in T data)
        {
            if (DidCallNext)
                throw new InvalidOperationException(SR.Handler_NextInvokedOnceOnly);

            DidCallNext = true;
            return invoker.InvokeContinuationTyped(data, invoker.Origin).Unwrap();
        }

        public EventResult GetEventResult()
            => result.HasValue ? new EventResult((object?)result.Value) : default;
    }
}
