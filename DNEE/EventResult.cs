using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DNEE
{
    /// <summary>
    /// The result of an event invocation.
    /// </summary>
    public struct EventResult : IEquatable<EventResult>
    {
        /// <summary>
        /// Gets whether or not this <see cref="EventResult"/> actually contains a value.
        /// </summary>
        public bool HasValue { get; }

        private readonly dynamic? result;
        /// <summary>
        /// Gets the result of the event invocation, if there is one.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <see langword="false"/>.</exception>
        public dynamic? Result => HasValue ? result : throw new InvalidOperationException();

        internal EventResult(dynamic? result)
        {
            HasValue = true;
            this.result = result;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => obj is EventResult er && Equals(er);

        /// <inheritdoc/>
        public bool Equals(EventResult other)
        {
            if (HasValue ^ other.HasValue) return false;
            if (HasValue) return result == other.result;
            else return true;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 1254623300;
            hashCode = hashCode * -1521134295 + HasValue.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<dynamic>.Default.GetHashCode(result);
            return hashCode;
        }

        /// <summary>
        /// Compares two <see cref="EventResult"/>s for equality.
        /// </summary>
        /// <param name="left">The first result to compare.</param>
        /// <param name="right">The second result to compare.</param>
        /// <returns><see langword="true"/> if they are equal, <see langword="false"/> otherwise.</returns>
        public static bool operator ==(EventResult left, EventResult right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="EventResult"/>s for inequality.
        /// </summary>
        /// <param name="left">The first result to compare.</param>
        /// <param name="right">The second result to compare.</param>
        /// <returns><see langword="true"/> if they are not equal, <see langword="false"/> otherwise.</returns>
        public static bool operator !=(EventResult left, EventResult right) => !(left == right);
    }

    /// <summary>
    /// The result of an event invocation with an expected return type.
    /// </summary>
    /// <typeparam name="T">The type the event was expected to return.</typeparam>
    public struct EventResult<T> : IEquatable<EventResult<T>>, IEquatable<EventResult>
    {
        /// <summary>
        /// Gets whether or not this <see cref="EventResult{T}"/> has a value.
        /// </summary>
        public bool HasValue { get; }
        /// <summary>
        /// Gets whether or not the value this <see cref="EventResult{T}"/> holds is strongly typed.
        /// </summary>
        public bool IsTyped { get; }

        private readonly T typedResult;
        /// <summary>
        /// Gets the strongly typed result of the event invocation, if there is one.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <see langword="false"/>
        /// -OR- the result is not of type <typeparamref name="T"/> and <see cref="IsTyped"/> is <see langword="false"/>.</exception>
        public T Result => HasValue && IsTyped ? typedResult : throw new InvalidOperationException();

        private readonly dynamic? dynamicResult;
        /// <summary>
        /// Gets the result of the event invocation, if there is one.
        /// </summary>
        /// <remarks>
        /// The object returned may be different than the one returned from <see cref="Result"/> if <see cref="IsTyped"/> is set.
        /// This can happen when this <see cref="EventResult{T}"/> was constructed with an object implementing <see cref="IUsableAs{T}"/>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <see langword="false"/>.</exception>
        public dynamic? DynamicResult => HasValue ? (IsTyped ? dynamicResult ?? typedResult : dynamicResult) : throw new InvalidOperationException();

        internal EventResult(T typedResult)
        {
            HasValue = true;
            IsTyped = true;
            this.typedResult = typedResult;
            dynamicResult = null;
        }
        internal EventResult(dynamic? dynResult)
        {
            HasValue = true;
            if (dynResult is T tval)
            {
                IsTyped = true;
                dynamicResult = dynResult;
                typedResult = tval;
            }
            else if (dynResult is IUsableAs<T> usable)
            {
                IsTyped = true;
                dynamicResult = dynResult;
                typedResult = usable.AsType;
            }
            else
            {
                IsTyped = false;
                dynamicResult = dynResult;
                typedResult = default!;
            }
        }

        /// <summary>
        /// Implicitly converts an <see cref="EventResult{T}"/> to an <see cref="EventResult"/>.
        /// </summary>
        /// <param name="er">The <see cref="EventResult{T}"/> to convert.</param>
        /// <seealso cref="ToEventResult"/>
        public static implicit operator EventResult(in EventResult<T> er)
            => er.ToEventResult();

        /// <summary>
        /// Converts the current <see cref="EventResult{T}"/> to an <see cref="EventResult"/>.
        /// </summary>
        /// <returns>The converted value.</returns>
        public EventResult ToEventResult()
            => HasValue ? new EventResult((object?)DynamicResult) : default;

        /// <summary>
        /// Implicitly converts an <see cref="EventResult"/> to an <see cref="EventResult{T}"/>.
        /// </summary>
        /// <param name="er">The <see cref="EventResult"/> to convert.</param>
        /// <seealso cref="FromEventResult(in EventResult)"/>
        public static implicit operator EventResult<T>(in EventResult er)
            => FromEventResult(er);


        /// <summary>
        /// Converts an <see cref="EventResult"/> to an <see cref="EventResult{T}"/>.
        /// </summary>
        /// <param name="er">The <see cref="EventResult"/> to convert.</param>
        /// <returns>The converted value.</returns>
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", 
            Justification = "No matter what, the generic type parameter will have to be specified.")]
        public static EventResult<T> FromEventResult(in EventResult er)
            => er.HasValue ? new EventResult<T>((object?)er.Result) : default;

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => (obj is EventResult<T> ert && Equals(ert))
            || (obj is EventResult er && Equals(er));

        /// <inheritdoc/>
        public bool Equals(EventResult<T> other)
        {
            if (HasValue != other.HasValue) return false;
            if (!HasValue) return true;
            if (IsTyped != other.IsTyped) return false;
            if (IsTyped) return EqualityComparer<T>.Default.Equals(Result, other.Result);
            else return DynamicResult == other.DynamicResult;
        }

        /// <inheritdoc/>
        public bool Equals(EventResult other)
        {
            if (HasValue != other.HasValue) return false;
            if (!HasValue) return true;
            if (IsTyped)
            {
                return other.Result is T otherRes && EqualityComparer<T>.Default.Equals(Result, otherRes);
            }
            else
            {
                return DynamicResult == other.Result;
            }
        }

        /// <summary>
        /// Compares two <see cref="EventResult{T}"/>s for equality.
        /// </summary>
        /// <param name="left">The first result to compare.</param>
        /// <param name="right">The second result to compare.</param>
        /// <returns><see langword="true"/> if they are equal, <see langword="false"/> otherwise.</returns>
        public static bool operator ==(EventResult<T> left, EventResult<T> right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="EventResult{T}"/>s for inequality.
        /// </summary>
        /// <param name="left">The first result to compare.</param>
        /// <param name="right">The second result to compare.</param>
        /// <returns><see langword="true"/> if they are not equal, <see langword="false"/> otherwise.</returns>
        public static bool operator !=(EventResult<T> left, EventResult<T> right) => !(left == right);

        /// <summary>
        /// Compares an <see cref="EventResult"/> and an <see cref="EventResult{T}"/> for equality.
        /// </summary>
        /// <param name="left">The first result to compare.</param>
        /// <param name="right">The second result to compare.</param>
        /// <returns><see langword="true"/> if they are equal, <see langword="false"/> otherwise.</returns>
        public static bool operator ==(EventResult<T> left, EventResult right) => left.Equals(right);

        /// <summary>
        /// Compares an <see cref="EventResult"/> and an <see cref="EventResult{T}"/> for inequality.
        /// </summary>
        /// <param name="left">The first result to compare.</param>
        /// <param name="right">The second result to compare.</param>
        /// <returns><see langword="true"/> if they are not equal, <see langword="false"/> otherwise.</returns>
        public static bool operator !=(EventResult<T> left, EventResult right) => !(left == right);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -1715740316;
            hashCode = hashCode * -1521134295 + HasValue.GetHashCode();
            hashCode = hashCode * -1521134295 + IsTyped.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(typedResult);
            hashCode = hashCode * -1521134295 + EqualityComparer<dynamic?>.Default.GetHashCode((object?)dynamicResult);
            return hashCode;
        }
    }
}