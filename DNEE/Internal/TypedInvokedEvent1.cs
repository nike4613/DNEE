using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    internal sealed class TypedInvokedEvent1<T> : IEvent<T>, IEventWithResult, IDataHistoryNode<T>
    {

        private readonly TypedInvoker1<T> invoker;

        public TypedInvokedEvent1(DataOrigin dataOrigin, in EventName name, TypedInvoker1<T> invoker, dynamic? data, IDataHistoryNode? last)
        {
            DataOrigin = dataOrigin;
            EventName = name;
            this.invoker = invoker;
            DynamicData = data;
            nextNode = last;
            typedDataHistory = new DataHistoryEnumerable<T>(this);
        }

        public EventName EventName { get; }

        private Maybe<dynamic?> result = Maybe.None;
        public dynamic? Result { set => result = Maybe.Some((object?)value); }
        public bool DidCallNext { get; private set; } = false;
        public bool AlwaysInvokeNext { get; set; } = true;
        public DataOrigin DataOrigin { get; }

        public dynamic? DynamicData { get; }

        public T Data => (T)(object?)DynamicData!;

        public DataOrigin Origin => DataOrigin;

        dynamic? IDataHistoryNode.Data => DynamicData;

        private readonly IDataHistoryNode? nextNode;
        IDataHistoryNode? IDataHistoryNode.Next => nextNode;

        public bool IsTyped => DynamicData is T;

        private readonly DataHistoryEnumerable<T> typedDataHistory;
        public IEnumerable<DataWithOrigin> DataHistory => typedDataHistory;
        IEnumerable<DataWithOrigin<T>> IEvent<T>.DataHistory => typedDataHistory;

        public EventResult Next(dynamic? data)
        {
            if (DidCallNext)
                throw new InvalidOperationException(SR.Handler_NextInvokedOnceOnly);

            DidCallNext = true;
            return invoker.InvokeContinuationDynamic((object?)data, invoker.Origin, this).Unwrap();
        }

        public EventResult Next(in T data)
        {
            if (DidCallNext)
                throw new InvalidOperationException(SR.Handler_NextInvokedOnceOnly);

            DidCallNext = true;
            return invoker.InvokeContinuationTyped(data, invoker.Origin, this).Unwrap();
        }

        public EventResult GetEventResult()
            => result.HasValue ? new EventResult((object?)result.Value) : default;
    }
}
