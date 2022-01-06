using DNEE.Internal.Resources;
using DNEE.Tuning;
using DNEE.Utility;
using System;
using System.Collections.Generic;

namespace DNEE.Internal
{
    internal struct TypedInvokedEvent1<T> : IEvent<T>, IInternalEventImpl<TypedInvokedEvent1<T>>
    {
        private readonly TypedInvoker1<T> invoker;

        public TypedInvokedEvent1(DataOrigin dataOrigin, in EventName name, TypedInvoker1<T> invoker, dynamic? data, ICreatedEvent? last)
        {
            DataOrigin = dataOrigin;
            EventName = name;
            this.invoker = invoker;
            DynamicData = data;
            DidCallNext = false;
            lazyData = null;
            lazyDataHistory = null;
            Holder = null;
            lastEvent = last;
        }

        public ICreatedEvent<TypedInvokedEvent1<T>>? Holder { get; set; }

        object IInternalEvent<TypedInvokedEvent1<T>>.InterfaceContract => InterfaceContract.Instance;

        void IInternalEvent<TypedInvokedEvent1<T>>.SetHolder(ICreatedEvent<TypedInvokedEvent1<T>> created)
            => Holder = created;

        private sealed class InterfaceContract : EventInterfaceContract<TypedInvokedEvent1<T>>
        {
            public static readonly InterfaceContract Instance = new();

            public override ICreatedEvent? GetLastEventCore(ref TypedInvokedEvent1<T> @event) => @event.lastEvent;
        }

        private readonly ICreatedEvent? lastEvent;

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

        public bool HasTypedData => GetData().HasValue;

        public DataOrigin Origin => DataOrigin;

        // TODO: this should probably be cleared during reset, but it won't live very long anyway, and *probably* wasn't used
        private DataHistoryEnumerable<T>? lazyDataHistory;
        public DataHistoryEnumerable<T> DataHistory => lazyDataHistory ??= new DataHistoryEnumerable<T>(Holder ?? throw new InvalidOperationException());
        IEnumerable<DataWithOrigin> IEvent.DataHistory => DataHistory;
        IEnumerable<DataWithOrigin<T>> IEvent<T>.DataHistory => DataHistory;

        public EventResult Next()
        {
            if (DidCallNext)
                throw new InvalidOperationException(SR.Handler_NextInvokedOnceOnly);

            DidCallNext = true;
            // we always invoke the dynamic continuation because it makes my life *much* simpler
            return invoker.InvokeContinuationDynamic((object?)DynamicData, DataOrigin, Holder).Unwrap();
        }

        public EventResult Next(dynamic? data)
        {
            if (DidCallNext)
                throw new InvalidOperationException(SR.Handler_NextInvokedOnceOnly);

            DidCallNext = true;
            return invoker.InvokeContinuationDynamic((object?)data, invoker.Origin, Holder).Unwrap();
        }

        public EventResult Next(in T data)
        {
            if (DidCallNext)
                throw new InvalidOperationException(SR.Handler_NextInvokedOnceOnly);

            DidCallNext = true;
            if ((object?)DynamicData is AssociatedData assoc)
            {
                assoc.AddData(data);
                return invoker.InvokeContinuationRelatedTyped(assoc, data, invoker.Origin, Holder).Unwrap();
            }
            return invoker.InvokeContinuationTyped(data, invoker.Origin, Holder).Unwrap();
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
                return invoker.InvokeContinuationRelatedTyped(assoc, usable, invoker.Origin, Holder).Unwrap();
            }
            return invoker.InvokeContinuationRelatedTyped(data, data.AsType, invoker.Origin, Holder).Unwrap();
        }

        public EventResult GetEventResult()
            => result.HasValue ? new EventResult((object?)result.Value) : default;
    }
}
