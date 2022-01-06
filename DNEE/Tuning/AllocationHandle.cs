using System;

namespace DNEE.Tuning
{
    /// <summary>
    /// A handle for an allocated object which, when disposed, calls the allocator's cleanup method, if provided.
    /// </summary>
    /// <typeparam name="T">The type of the object which was allocated.</typeparam>
    public readonly struct AllocationHandle<T> : IDisposable
    {
        /// <summary>
        /// Gets the allocated object.
        /// </summary>
        public T Object { get; }

        private readonly Action<T>? cleanup;

        /// <summary>
        /// Constructs an <see cref="AllocationHandle{T}"/> for the provided object, using the provided cleanup method.
        /// </summary>
        /// <param name="object">The object which was allocated.</param>
        /// <param name="cleanup">The cleanup method to call on disposal.</param>
        public AllocationHandle(T @object, Action<T>? cleanup)
        {
            Object = @object;
            this.cleanup = cleanup;
        }

        /// <summary>
        /// Disposes this handle, cleaning up the allocated object, according to the allocator.
        /// </summary>
        public void Dispose() => cleanup?.Invoke(Object);
    }
}
