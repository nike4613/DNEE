namespace DNEE.Tuning
{
    public interface IEventAllocator
    {
        AllocationHandle<IAllocatedEvent<TImpl>> AllocateTypeless<TImpl>(TImpl impl) where TImpl : IEvent, IInternalEvent<TImpl>;

        AllocationHandle<IAllocatedEvent<TImpl, T>> AllocateInTyped<TImpl, T>(TImpl impl) where TImpl : IEvent<T>, IInternalEvent<TImpl>;

        AllocationHandle<IAllocatedEvent<TImpl, T, TRet>> AllocateInOutTyped<TImpl, T, TRet>(TImpl impl) where TImpl : IEvent<T, TRet>, IInternalEvent<TImpl>;
    }

    public interface IAllocatedEvent<TImpl> : IEvent, ICreatedEvent<TImpl>
        where TImpl : IEvent, IInternalEvent<TImpl>
    {
    }

    public interface IAllocatedEvent<TImpl, T> : IEvent<T>, ICreatedEvent<TImpl>
        where TImpl : IEvent<T>, IInternalEvent<TImpl>
    {
    }

    public interface IAllocatedEvent<TImpl, T, TRet> : IEvent<T, TRet>, ICreatedEvent<TImpl>
        where TImpl : IEvent<T, TRet>, IInternalEvent<TImpl>
    {
    } 
}
