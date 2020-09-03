using System;
using System.Collections.Generic;

namespace BSEventsSystem
{
    public struct EventResult : IEquatable<EventResult>
    {
        public bool HasValue { get; }

        private readonly dynamic? result;
        public dynamic? Result => HasValue ? result : throw new InvalidOperationException();

        internal EventResult(dynamic? result)
        {
            HasValue = true;
            this.result = result;
        }

        public bool Equals(EventResult other)
        {
            if (HasValue ^ other.HasValue) return false;
            if (HasValue) return result == other.result;
            else return true;
        }
    }

    public struct EventResult<T> : IEquatable<EventResult<T>>, IEquatable<EventResult>
    {
        public bool HasValue { get; }
        public bool IsTyped { get; }

        private readonly T typedResult;
        public T Result => HasValue && IsTyped ? typedResult : throw new InvalidOperationException();

        private readonly dynamic? dynamicResult;
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

        public static implicit operator EventResult(in EventResult<T> er)
            => er.HasValue ? new EventResult(er.DynamicResult) : default;

        public bool Equals(EventResult<T> other)
        {
            if (HasValue != other.HasValue) return false;
            if (!HasValue) return true;
            if (IsTyped != other.IsTyped) return false;
            if (IsTyped) return EqualityComparer<T>.Default.Equals(Result, other.Result);
            else return DynamicResult == other.DynamicResult;
        }

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