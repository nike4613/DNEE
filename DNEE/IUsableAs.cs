using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE
{
    /// <summary>
    /// An interface that a type can implement to make it transparently useable as <typeparamref name="T"/>
    /// when used as event data.
    /// </summary>
    /// <typeparam name="T">The type that the implementer can be used as.</typeparam>
    public interface IUsableAs<T>
    {
        /// <summary>
        /// Gets the implementing object as type <typeparamref name="T"/> (with either reference or value semantics, defined by the type).
        /// </summary>
        T AsType { get; }
    }
}
