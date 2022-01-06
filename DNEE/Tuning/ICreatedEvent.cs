namespace DNEE.Tuning
{
    /// <summary>
    /// An allocated event object, fully type erased.
    /// </summary>
    public interface ICreatedEvent
    {
        /// <summary>
        /// Implementers must forward this to <see cref="IInternalEvent{TSelf}.InterfaceContract"/>.
        /// </summary>
        object InterfaceContract { get; }
    }

    /// <summary>
    /// An allocated event object, providing access to the actual internal event object.
    /// </summary>
    /// <typeparam name="TEvent">The type of the internal event object.</typeparam>
    public interface ICreatedEvent<TEvent> : ICreatedEvent where TEvent : IInternalEvent<TEvent>
    {
        /// <summary>
        /// Gets a reference to the stored internal event object.
        /// </summary>
        /// <returns>A reference to the stored internal event object.</returns>
        ref TEvent GetInternalEvent();
    }
}
