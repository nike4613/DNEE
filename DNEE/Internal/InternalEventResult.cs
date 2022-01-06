using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace DNEE.Internal
{
    internal struct InternalEventResult
    {
        public EventResult Result { get; }
        public ExceptionDispatchInfo? Exception { get; }

        public InternalEventResult(EventResult result, ExceptionDispatchInfo? exception)
        {
            Exception = exception;
            Result = result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // to try to prevent this from showing up in stack traces
        public EventResult Unwrap()
        {
            if (Exception != null)
                Exception.Throw();
            return Result;
        }

        public static ExceptionDispatchInfo? CombineExceptions(ExceptionDispatchInfo? first, ExceptionDispatchInfo? second)
        {
            if (first is null) return second;
            if (second is null) return first;

            var aggregate = new AggregateException(first.SourceException, second.SourceException);
            // TODO: maybe flatten?

            return ExceptionDispatchInfo.Capture(aggregate);
        }
    }

    internal struct InternalEventResult<T>
    {
        public EventResult<T> Result { get; }
        public ExceptionDispatchInfo? Exception { get; }

        public InternalEventResult(EventResult result, ExceptionDispatchInfo? exception)
        {
            Exception = exception;
            Result = result;
        }

        public static implicit operator InternalEventResult<T>(InternalEventResult r) => new(r.Result, r.Exception);
        public static implicit operator InternalEventResult(InternalEventResult<T> r) => new(r.Result, r.Exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // to try to prevent this from showing up in stack traces
        public EventResult<T> Unwrap()
        {
            if (Exception != null)
                Exception.Throw();
            return Result;
        }
    }
}
