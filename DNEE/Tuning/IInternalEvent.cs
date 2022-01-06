using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Tuning
{
    /// <summary>
    /// An internal event object of type <typeparamref name="TSelf"/>.
    /// </summary>
    /// <typeparam name="TSelf">The type of this internal event object.</typeparam>
    public interface IInternalEvent<TSelf> : IEvent where TSelf : IInternalEvent<TSelf>
    {
        /// <summary>
        /// Gets the interface contract object for this event.
        /// </summary>
        object InterfaceContract { get; }
        /// <summary>
        /// Sets the owning allocated object on the event object.
        /// </summary>
        /// <remarks>
        /// This must be called after the event object has been copied into its final location
        /// on the heap.
        /// </remarks>
        /// <param name="created">The allocated event object.</param>
        void SetHolder(ICreatedEvent<TSelf> created);
    }
}
