using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE
{
    /// <summary>
    /// A wrapper for an <see cref="IEvent"/>.
    /// </summary>
    /// <remarks>
    /// Any type which wraps an <see cref="IEvent"/> should implement this to allow DNEE to correctly
    /// retrieve the event that it provided when necessary.
    /// </remarks>
    public interface IEventWrapper
    {
        /// <summary>
        /// Gets the event which this wrapper wraps.
        /// </summary>
        IEvent BaseEvent { get; }
    }
}
