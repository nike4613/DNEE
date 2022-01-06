namespace DNEE.Tuning
{
    /// <summary>
    /// An allocator which creates heap-based implementation objects for the event interfaces, based on some implementation type.
    /// </summary>
    public interface IEventAllocator
    {
        /// <summary>
        /// Allocates an event object for the provided implementation object.
        /// </summary>
        /// <remarks>
        /// <para>The allocated object must implement all event interface members as direct pass-through to the implementation object.
        /// Members of <see cref="ICreatedEvent{TImpl}"/> must be implemented as documented there.</para>
        /// <para>After the object has copied <paramref name="impl"/> onto the heap, it must call
        /// <see cref="IInternalEvent{TSelf}.SetHolder(ICreatedEvent{TSelf})"/> with itself.</para>
        /// </remarks>
        /// <typeparam name="TImpl">The type of the underlying implementation object.</typeparam>
        /// <param name="impl">The implementation object to allocate.</param>
        /// <returns>An <see cref="AllocationHandle{T}"/> for the allocated object.</returns>
        AllocationHandle<IAllocatedEvent<TImpl>> AllocateTypeless<TImpl>(TImpl impl) where TImpl : IEvent, IInternalEvent<TImpl>;

        /// <summary>
        /// Allocates an event object for the provided implementation object.
        /// </summary>
        /// <remarks>
        /// <para>The allocated object must implement all event interface members as direct pass-through to the implementation object.
        /// Members of <see cref="ICreatedEvent{TImpl}"/> must be implemented as documented there.</para>
        /// <para>After the object has copied <paramref name="impl"/> onto the heap, it must call
        /// <see cref="IInternalEvent{TSelf}.SetHolder(ICreatedEvent{TSelf})"/> with itself.</para>
        /// </remarks>
        /// <typeparam name="TImpl">The type of the underlying implementation object.</typeparam>
        /// <typeparam name="T">The type of the value assocated with the <see cref="IEvent{T}"/>.</typeparam>
        /// <param name="impl">The implementation object to allocate.</param>
        /// <returns>An <see cref="AllocationHandle{T}"/> for the allocated object.</returns>
        AllocationHandle<IAllocatedEvent<TImpl, T>> AllocateInTyped<TImpl, T>(TImpl impl) where TImpl : IEvent<T>, IInternalEvent<TImpl>;

        /// <summary>
        /// Allocates an event object for the provided implementation object.
        /// </summary>
        /// <remarks>
        /// <para>The allocated object must implement all event interface members as direct pass-through to the implementation object.
        /// Members of <see cref="ICreatedEvent{TImpl}"/> must be implemented as documented there.</para>
        /// <para>After the object has copied <paramref name="impl"/> onto the heap, it must call
        /// <see cref="IInternalEvent{TSelf}.SetHolder(ICreatedEvent{TSelf})"/> with itself.</para>
        /// </remarks>
        /// <typeparam name="TImpl">The type of the underlying implementation object.</typeparam>
        /// <typeparam name="T">The type of the value assocated with the <see cref="IEvent{T, TRet}"/>.</typeparam>
        /// <typeparam name="TRet">The type of the return value associated with the <see cref="IEvent{T, TRet}"/>.</typeparam>
        /// <param name="impl">The implementation object to allocate.</param>
        /// <returns>An <see cref="AllocationHandle{T}"/> for the allocated object.</returns>
        AllocationHandle<IAllocatedEvent<TImpl, T, TRet>> AllocateInOutTyped<TImpl, T, TRet>(TImpl impl) where TImpl : IEvent<T, TRet>, IInternalEvent<TImpl>;
    }

    /// <summary>
    /// A helper interface implementing <see cref="IEvent"/> and <see cref="ICreatedEvent{TImpl}"/>.
    /// </summary>
    /// <typeparam name="TImpl">The implementation type of the internal event.</typeparam>
    public interface IAllocatedEvent<TImpl> : IEvent, ICreatedEvent<TImpl>
        where TImpl : IEvent, IInternalEvent<TImpl>
    {
    }

    /// <summary>
    /// A helper interface implementing <see cref="IEvent{T}"/> and <see cref="ICreatedEvent{TEvent}"/>.
    /// </summary>
    /// <typeparam name="TImpl">The implementation type of the internal event.</typeparam>
    /// <typeparam name="T">The type of the value assocated with the <see cref="IEvent{T}"/>.</typeparam>
    public interface IAllocatedEvent<TImpl, T> : IEvent<T>, ICreatedEvent<TImpl>
        where TImpl : IEvent<T>, IInternalEvent<TImpl>
    {
    }

    /// <summary>
    /// A helper interface implementing <see cref="IEvent{T, TRet}"/> and <see cref="ICreatedEvent{TEvent}"/>.
    /// </summary>
    /// <typeparam name="TImpl">The implementation type of the internal event.</typeparam>
    /// <typeparam name="T">The type of the value assocated with the <see cref="IEvent{T, TRet}"/>.</typeparam>
    /// <typeparam name="TRet">The type of the return value associated with the <see cref="IEvent{T, TRet}"/>.</typeparam>
    public interface IAllocatedEvent<TImpl, T, TRet> : IEvent<T, TRet>, ICreatedEvent<TImpl>
        where TImpl : IEvent<T, TRet>, IInternalEvent<TImpl>
    {
    } 
}
