using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace DNEE.Utility
{
    /// <summary>
    /// A convenience type for creating instances of <see cref="Maybe{T}"/>.
    /// </summary>
    public sealed class Maybe
    {
        private Maybe() { }

        /// <summary>
        /// Creates a new <see cref="Maybe{T}"/> with <paramref name="value"/> as its value.
        /// </summary>
        /// <typeparam name="T">The type to wrap.</typeparam>
        /// <param name="value">The value to wrap.</param>
        /// <returns>A <see cref="Maybe{T}"/> wrapping <paramref name="value"/>.</returns>
        /// <seealso cref="Maybe{T}.Maybe(T)"/>
        public static Maybe<T> Some<T>(T value) => new Maybe<T>(value);
        /// <summary>
        /// A value that can be implicitly converted to a <see cref="Maybe{T}"/> with no value.
        /// </summary>
        /// <seealso cref="Maybe{T}.None"/>
        public static Maybe None { get; } = new Maybe();
    }

    /// <summary>
    /// A wrapper type that wraps a value of type <typeparamref name="T"/>, allowing for a distinction
    /// between no value and a <see langword="null"/> value.
    /// </summary>
    /// <typeparam name="T">The type to wrap.</typeparam>
    /// <see cref="Maybe"/>
    [DebuggerDisplay("{DebuggerView,nq}")]
    public readonly struct Maybe<T> : IEquatable<Maybe<T>>, IEquatable<T>
    {
        private string DebuggerView => HasValue ? $"Some({Value})" : "None";

        private readonly T value;

        /// <summary>
        /// Gets the value wrapped by this <see cref="Maybe{T}"/>, if present.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if this <see cref="Maybe{T}"/> does not contain a value.</exception>
        /// <seealso cref="HasValue"/>
        public T Value => HasValue ? value : throw new InvalidOperationException();
        /// <summary>
        /// Gets whether or not this <see cref="Maybe{T}"/> contains a value.
        /// </summary>
        public bool HasValue { get; }


        /// <summary>
        /// Gets the empty <see cref="Maybe{T}"/>.
        /// </summary>
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", 
            Justification = "All uses of this can be replaced by Maybe.None (which implicitly converts for nice type inference)")]
        public static Maybe<T> None => default;

        /// <summary>
        /// Constructs a <see cref="Maybe{T}"/> wrapping <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <seealso cref="Maybe.Some{T}(T)"/>
        public Maybe(T value)
        {
            this.value = value;
            HasValue = true;
        }

        /// <summary>
        /// Gets the stored value, or the value specified as <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to use if there is no specified value.</param>
        /// <returns><see cref="Value"/>, if present, otherwise <paramref name="value"/>.</returns>
        public T ValueOr(T value)
            => HasValue ? Value : value;

        /// <summary>
        /// Checks if this <see cref="Maybe{T}"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The value to compare to.</param>
        /// <returns><see langword="true"/> if they are equal, <see langword="false"/> otherwise.</returns>
        public bool Equals(Maybe<T> other)
        {
            if (HasValue ^ other.HasValue) return false;
            return !HasValue || EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        /// <summary>
        /// Checks if the value wrapped by this <see cref="Maybe{T}"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The value to compare to.</param>
        /// <returns><see langword="true"/> if they are equal, <see langword="false"/> otherwise.</returns>
        public bool Equals(T other)
        {
            if (!HasValue) return false;
            return EqualityComparer<T>.Default.Equals(Value, other);
        }

        /// <summary>
        /// Checks if two <see cref="Maybe{T}"/>s are equal.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns><see langword="true"/> if they are equal, <see langword="false"/> otherwise.</returns>
        public static bool operator ==(Maybe<T> a, Maybe<T> b) => a.Equals(b);
        /// <summary>
        /// Checks if two <see cref="Maybe{T}"/>s are not equal.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns><see langword="true"/> if they are not equal, <see langword="false"/> otherwise.</returns>
        public static bool operator !=(Maybe<T> a, Maybe<T> b) => !(a == b);


        /// <summary>
        /// Wraps a value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <seealso cref="Maybe{T}.Maybe(T)"/>
        /// <seealso cref="Maybe.Some{T}(T)"/>
        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", 
            Justification = "The named alternative is the constructor.")]
        public static explicit operator Maybe<T>(T value) => new Maybe<T>(value);


        /// <summary>
        /// Implicitly converts the result of <see cref="Maybe.None"/> into <see cref="None"/>.
        /// </summary>
        /// <param name="_"><see cref="Maybe.None"/>.</param>
        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", 
            Justification = "This exists solely for type inference of the form Maybe.None.")]
        public static implicit operator Maybe<T>(Maybe _) => None;

        /// <summary>
        /// Stringifies the wrapped value, if available.
        /// </summary>
        /// <returns>The string representation of the wrapped value, if available.</returns>
        public override string ToString()
            => HasValue ? value?.ToString() ?? "" : "";

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => (obj is T t && Equals(t))
            || (obj is Maybe<T> mt && Equals(mt));

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 1816676634;
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + HasValue.GetHashCode();
            return hashCode;
        }
    }
}
