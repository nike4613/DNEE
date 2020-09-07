using System;
using System.Collections.Generic;

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
        public bool Equals(EventResult other)
        {
            if (HasValue ^ other.HasValue) return false;
            if (HasValue) return result == other.result;
            else return true;
        }
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
        /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <see langword="false"/>.</exception>
        public dynamic? DynamicResult => HasValue ? (IsTyped ? typedResult : dynamicResult) : throw new InvalidOperationException();

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
                dynamicResult = null;
                typedResult = tval;
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
        public static implicit operator EventResult(in EventResult<T> er)
            => er.HasValue ? new EventResult((object?)er.DynamicResult) : default;

        /// <summary>
        /// Implicitly converts an <see cref="EventResult"/> to an <see cref="EventResult{T}"/>.
        /// </summary>
        /// <param name="er">The <see cref="EventResult"/> to convert.</param>
        public static implicit operator EventResult<T>(in EventResult er)
            => er.HasValue ? new EventResult<T>((object?)er.Result) : default;

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
    }
}