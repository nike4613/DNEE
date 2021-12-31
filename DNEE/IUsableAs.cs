using System.Diagnostics.CodeAnalysis;

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

    /// <summary>
    /// An interfacea type can implement to make it transparently usable as some type, as defined by
    /// the result of <see cref="TryAsType{T}(out T)"/>.
    /// </summary>
    public interface IDynamicallyUsableAs
    {
        /// <summary>
        /// Tries to get the implementing object as type <typeparamref name="T"/>, if possible.
        /// </summary>
        /// <typeparam name="T">The type that the implementer may be able to be used as.</typeparam>
        /// <param name="value">The implementing object, as the requested type, if any.</param>
        /// <returns></returns>
        bool TryAsType<T>([MaybeNullWhen(false)] out T value);
    }
}
