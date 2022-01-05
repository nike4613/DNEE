using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Tuning
{
    public sealed class DefaultEventAllocator : IEventAllocator
    {
        public static readonly DefaultEventAllocator Instance = new();

        private abstract class WrapperBase<TImpl> : ICreatedEvent<TImpl>
            where TImpl : IInternalEvent<TImpl>
        {
            // mutable struct, not readonly
            protected TImpl Impl;

            public WrapperBase(TImpl impl)
            {
                Impl = impl;
                Impl.SetHolder(this);
            }

            ICreatedEvent? ICreatedEvent.LastEvent => Impl.LastEvent;
            ref TImpl ICreatedEvent<TImpl>.GetInternalEvent() => ref Impl;

        }

        private class TypelessWrapper<TImpl> : WrapperBase<TImpl>, IEvent
            where TImpl : IEvent, IInternalEvent<TImpl>
        {
            public TypelessWrapper(TImpl impl) : base(impl) { }

            DataOrigin IEvent.DataOrigin => Impl.DataOrigin;
            EventName IEvent.EventName => Impl.EventName;
            dynamic? IEvent.Result { get => Impl.Result; set => Impl.Result = value; }
            dynamic? IEvent.Data => Impl.Data;
            bool IEvent.AlwaysInvokeNext { get => Impl.AlwaysInvokeNext; set => Impl.AlwaysInvokeNext = value; }
            IEnumerable<DataWithOrigin> IEvent.DataHistory => Impl.DataHistory;
            EventResult IEvent.Next() => Impl.Next();
            EventResult IEvent.Next(dynamic? data) => Impl.Next((object?)data);
        }

        public AllocationHandle<IEvent> AllocateTypeless<TImpl>(TImpl impl) where TImpl : IEvent, IInternalEvent<TImpl>
            => new(new TypelessWrapper<TImpl>(impl), null);

        private class InTypedWrapper<TImpl, T> : TypelessWrapper<TImpl>, IEvent<T>
            where TImpl : IEvent<T>, IInternalEvent<TImpl>
        {
            public InTypedWrapper(TImpl impl) : base(impl) { }

            dynamic? IEvent<T>.DynamicData => Impl.DynamicData;
            T IEvent<T>.Data => Impl.Data;
            IEnumerable<DataWithOrigin<T>> IEvent<T>.DataHistory => Impl.DataHistory;
            EventResult IEvent<T>.Next(in T data) => Impl.Next(data);
            EventResult IEvent<T>.Next(IUsableAs<T> data) => Impl.Next(data);
        }

        public AllocationHandle<IEvent<T>> AllocateInTyped<TImpl, T>(TImpl impl) where TImpl : IEvent<T>, IInternalEvent<TImpl>
            => new(new InTypedWrapper<TImpl, T>(impl), null);

        private class InOutTypedWrapper<TImpl, T, TRet> : InTypedWrapper<TImpl, T>, IEvent<T, TRet>
            where TImpl : IEvent<T, TRet>, IInternalEvent<TImpl>
        {
            public InOutTypedWrapper(TImpl impl) : base(impl) { }

            TRet IEvent<T, TRet>.Result { get => Impl.Result; set => Impl.Result = value; }
            EventResult<TRet> IEvent<T, TRet>.Next() => Impl.Next();
            EventResult<TRet> IEvent<T, TRet>.Next(in T data) => Impl.Next(data);
            EventResult<TRet> IEvent<T, TRet>.Next(IUsableAs<T> data) => Impl.Next(data);
        }

        public AllocationHandle<IEvent<T, TRet>> AllocateInOutTyped<TImpl, T, TRet>(TImpl impl) where TImpl : IEvent<T, TRet>, IInternalEvent<TImpl>
            => new(new InOutTypedWrapper<TImpl, T, TRet>(impl), null);
    }
}
