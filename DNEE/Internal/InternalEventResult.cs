using System;
using System.Collections.Generic;
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

        public EventResult Unwrap()
        {
            if (Exception != null)
                Exception.Throw();
            return Result;
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

        public static implicit operator InternalEventResult<T>(InternalEventResult r)
            => new InternalEventResult<T>(r.Result, r.Exception);
        public static implicit operator InternalEventResult(InternalEventResult<T> r)
            => new InternalEventResult(r.Result, r.Exception);

        public EventResult<T> Unwrap()
        {
            if (Exception != null)
                Exception.Throw();
            return Result;
        }
    }
}
