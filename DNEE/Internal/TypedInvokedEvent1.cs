using DNEE.Internal.Resources;
using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    internal sealed class TypedInvokedEvent1<T> : IEvent<T>, IDataHistoryNode<T>
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

        // TODO: make this less messy somehow

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

        dynamic? IEvent.Data => DynamicData;
        public dynamic? DynamicData { get; }

        private Maybe<T>? lazyData;

        private Maybe<T> GetData()
        {
            lazyData ??= Helpers.TryUseAs<T>((object?)DynamicData, out var value)
                ? Maybe.Some(value) : Maybe.None;
            return lazyData.Value;
        }

        public T Data
        {
            get
            {
                var data = GetData();
                if (data.HasValue)
                    return data.Value;
                throw new InvalidCastException();
            }
        }

        public bool IsTyped => GetData().HasValue;

        public DataOrigin Origin => DataOrigin;

        dynamic? IDataHistoryNode.Data => DynamicData;

        private readonly IDataHistoryNode? nextNode;
        IDataHistoryNode? IDataHistoryNode.Next => nextNode;

        private readonly DataHistoryEnumerable<T> typedDataHistory;
        public IEnumerable<DataWithOrigin> DataHistory => typedDataHistory;
        IEnumerable<DataWithOrigin<T>> IEvent<T>.DataHistory => typedDataHistory;

        public EventResult Next()
        {
            if (DidCallNext)
                throw new InvalidOperationException(SR.Handler_NextInvokedOnceOnly);

            DidCallNext = true;
            // we always invoke the dynamic continuation because it makes my life *much* simpler
            return invoker.InvokeContinuationDynamic((object?)DynamicData, DataOrigin, this).Unwrap();
        }

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
            if ((object?)DynamicData is AssociatedData assoc)
            {
                assoc.AddData(data);
                return invoker.InvokeContinuationRelatedTyped(assoc, data, invoker.Origin, this).Unwrap();
            }
            return invoker.InvokeContinuationTyped(data, invoker.Origin, this).Unwrap();
        }

        public EventResult Next(IUsableAs<T> data)
        {
            if (DidCallNext)
                throw new InvalidOperationException(SR.Handler_NextInvokedOnceOnly);

            DidCallNext = true;
            if ((object?)DynamicData is AssociatedData assoc)
            {
                var usable = data.AsType;
                assoc.AddData(usable);
                return invoker.InvokeContinuationRelatedTyped(assoc, usable, invoker.Origin, this).Unwrap();
            }
            return invoker.InvokeContinuationRelatedTyped(data, data.AsType, invoker.Origin, this).Unwrap();
        }

        public EventResult GetEventResult()
            => result.HasValue ? new EventResult((object?)result.Value) : default;
    }
}
