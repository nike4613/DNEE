using DNEE.Internal.Resources;
using DNEE.Tuning;
using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DNEE.Internal
{
    internal struct TypedInvokedEvent2<T, R> : IEvent<T, R>, IInternalEventImpl<TypedInvokedEvent2<T, R>>
    {
        private readonly TypedInvoker2<T, R> invoker;

        public TypedInvokedEvent2(DataOrigin dataOrigin, in EventName name, TypedInvoker2<T, R> invoker, dynamic? data, ICreatedEvent? last)
        {
            DataOrigin = dataOrigin;
            EventName = name;
            this.invoker = invoker;
            DynamicData = data;
            lastEvent = last;
            lazyDataHistory = null;
            Holder = null;
            DidCallNext = false;
            lazyData = null;
        }

        public ICreatedEvent<TypedInvokedEvent2<T, R>>? Holder { get; set; }
        object IInternalEvent<TypedInvokedEvent2<T, R>>.InterfaceContract => InterfaceContract.Instance;
        void IInternalEvent<TypedInvokedEvent2<T, R>>.SetHolder(ICreatedEvent<TypedInvokedEvent2<T, R>> created) => Holder = created;

        private sealed class InterfaceContract : EventInterfaceContract<TypedInvokedEvent2<T, R>>
        {
            public static readonly InterfaceContract Instance = new();

            public override ICreatedEvent? GetLastEventCore(ref TypedInvokedEvent2<T, R> @event) => @event.lastEvent;
        }

        private readonly ICreatedEvent? lastEvent;

        public EventName EventName { get; }

        public bool DidCallNext { get; private set; }
        public bool AlwaysInvokeNext { get; set; } = true;

        dynamic? IEvent.Data => DynamicData;

        public dynamic? DynamicData { get; }

        private Maybe<dynamic?> result = Maybe.None;
        dynamic? IEvent.Result
        {
            get => result.HasValue
                ? result.Value
                : typedResult.HasValue
                ? typedResult.Value
                : null;
            set
            {
                result = Maybe.Some((object?)value);
                typedResult = Maybe.None;
            }
        }

        private Maybe<R> typedResult = Maybe.None;
        R IEvent<T, R>.Result
        {
            get => typedResult.ValueOr(result.HasValue && result.Value is R r ? r : default!);
            set
            {
                typedResult = Maybe.Some(value);
                result = Maybe.None;
            }
        }

        // TODO: make this less messy somehow
        public DataOrigin DataOrigin { get; }

        private readonly DataHistoryEnumerable<T>? lazyDataHistory;
        public DataHistoryEnumerable<T> DataHistory => throw new NotImplementedException(); // TODO:
        IEnumerable<DataWithOrigin<T>> IEvent<T>.DataHistory => DataHistory;
        IEnumerable<DataWithOrigin> IEvent.DataHistory => DataHistory;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        EventResult IEvent.Next()
            => Next();

        public EventResult<R> Next()
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        EventResult IEvent<T>.Next(in T data)
            => Next(data);

        public EventResult<R> Next(in T data)
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        EventResult IEvent<T>.Next(IUsableAs<T> data)
            => Next(data);

        public EventResult<R> Next(IUsableAs<T> data)
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

        public EventResult<R> GetEventResult()
        {
            if (typedResult.HasValue)
                return new EventResult<R>(typedResult.Value);
            if (result.HasValue)
                return new EventResult<R>((object?)result.Value);
            return default;
        }
    }
}
