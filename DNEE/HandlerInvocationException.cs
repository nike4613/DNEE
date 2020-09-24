using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE
{
    /// <summary>
    /// An exception that is thrown whenever the event handlers for an event throw.
    /// </summary>
    public class HandlerInvocationException : Exception
    {
        /// <summary>
        /// Constructs a <see cref="HandlerInvocationException"/> with the given <paramref name="message"/> and <paramref name="innerException"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public HandlerInvocationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc/>
        public HandlerInvocationException()
        {
        }

        /// <inheritdoc/>
        public HandlerInvocationException(string message) : base(message)
        {
        }
    }
}
