using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    internal sealed class TypedInvokedEvent2<T, R> : IEvent<T, R>, IEventWithResult<R>
    {

        private readonly TypedInvoker2<T, R> invoker;

        public TypedInvokedEvent2(in EventName name, TypedInvoker2<T, R> invoker, dynamic? data)
        {
            EventName = name;
            this.invoker = invoker;
            DynamicData = data;
        }

        public EventName EventName { get; }

        public bool DidCallNext { get; private set; } = false;
        public bool AlwaysInvokeNext { get; set; } = true;

        public dynamic? DynamicData { get; }

        private Maybe<dynamic?> result = Maybe.None;
        dynamic? IEvent.Result
        {
            set
            {
                result = Maybe.Some((object?)value);
                typedResult = Maybe.None;
            }
        }

        private Maybe<R> typedResult = Maybe.None;
        R IEvent<T, R>.Result
        {
            set
            {
                typedResult = Maybe.Some(value);
                result = Maybe.None;
            }
        }

        public EventResult Next(dynamic? data)
        {
            if (DidCallNext)
                throw new InvalidOperationException(SR.Handler_NextInvokedOnceOnly);

            DidCallNext = true;
            return invoker.InvokeContinuationDynamic((object?)data).Unwrap();
        }

        EventResult IEvent<T>.Next(in T data)
            => Next(data);

        public EventResult<R> Next(in T data)
        {
            if (DidCallNext)
                throw new InvalidOperationException(SR.Handler_NextInvokedOnceOnly);

            DidCallNext = true;
            return invoker.InvokeContinuationTyped(data).Unwrap();
        }

        public EventResult<R> GetEventResult()
        {
            if (typedResult.HasValue)
                return new EventResult<R>(typedResult.Value);
            if (result.HasValue)
                return new EventResult<R>((object?)result.Value);
            return default;
        }

        EventResult IEventWithResult.GetEventResult() => GetEventResult();
    }
}
