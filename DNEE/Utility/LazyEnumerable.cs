using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Utility
{
    /// <summary>
    /// An enumerable which lazily gets an enumerable to actually enumerate, whenever requested.
    /// </summary>
    /// <remarks>
    /// This type's <see cref="GetEnumerator"/> method is implemented equivalently to 
    /// <c>func().GetEnumerator()</c>.
    /// </remarks>
    /// <typeparam name="T">The type to enumerate.</typeparam>
    public sealed class LazyEnumerable<T> : IEnumerable<T>
    {
        private readonly Func<IEnumerable<T>> getter;

        /// <summary>
        /// Constructs a <see cref="LazyEnumerable{T}"/> with the given delegate to provide
        /// the enumeration.
        /// </summary>
        /// <param name="func">The function which provides the underlying <see cref="IEnumerable{T}"/> to enumerate.</param>
        public LazyEnumerable(Func<IEnumerable<T>> func)
        {
            if (func is null)
                throw new ArgumentNullException(nameof(func));

            getter = func;
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
            => getter().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
             => GetEnumerator();
    }
}
