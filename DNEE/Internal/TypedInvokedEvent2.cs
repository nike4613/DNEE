using DNEE.Internal.Resources;
using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DNEE.Internal
{
    internal sealed class TypedInvokedEvent2<T, R> : IEvent<T, R>, IEventWithResult<R>, IDataHistoryNode<T>
    {

        private readonly TypedInvoker2<T, R> invoker;

        public TypedInvokedEvent2(DataOrigin dataOrigin, in EventName name, TypedInvoker2<T, R> invoker, dynamic? data, IDataHistoryNode? last)
        {
            DataOrigin = dataOrigin;
            EventName = name;
            this.invoker = invoker;
            DynamicData = data;
            nextNode = last;
            typedDataHistory = new DataHistoryEnumerable<T>(this);
        }

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

        private readonly DataHistoryEnumerable<T> typedDataHistory;
        public IEnumerable<DataWithOrigin<T>> DataHistory => typedDataHistory;

        IEnumerable<DataWithOrigin> IEvent.DataHistory => typedDataHistory;

        public bool IsTyped => DynamicData is T || DynamicData is IUsableAs<T>;

        public T Data
        {
            get
            {
                if (DynamicData is T tval)
                    return tval;
                if (DynamicData is IUsableAs<T> usable)
                    return usable.AsType;
                throw new InvalidCastException();
            }
        }

        public DataOrigin Origin => DataOrigin;

        dynamic? IDataHistoryNode.Data => DynamicData;

        private readonly IDataHistoryNode? nextNode;
        IDataHistoryNode? IDataHistoryNode.Next => nextNode;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        EventResult IEvent.Next()
            => Next();

        public EventResult<R> Next()
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
                return invoker.InvokeContinuationRelatedTyped(assoc, data, invoker.Origin, this).Unwrap();
            }
            return invoker.InvokeContinuationTyped(data, invoker.Origin, this).Unwrap();
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
                return invoker.InvokeContinuationRelatedTyped(assoc, usable, invoker.Origin, this).Unwrap();
            }
            return invoker.InvokeContinuationRelatedTyped(data, data.AsType, invoker.Origin, this).Unwrap();
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
