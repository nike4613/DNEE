using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Tuning
{
    public interface IEventAllocator
    {
        AllocationHandle<IEvent> AllocateTypeless<TImpl>(TImpl impl) where TImpl : IEvent, IInternalEvent<TImpl>;

        AllocationHandle<IEvent<T>> AllocateInTyped<TImpl, T>(TImpl impl) where TImpl : IEvent<T>, IInternalEvent<TImpl>;

        AllocationHandle<IEvent<T, TRet>> AllocateInOutTyped<TImpl, T, TRet>(TImpl impl) where TImpl : IEvent<T, TRet>, IInternalEvent<TImpl>;
    }
}
