using DNEE.Tuning;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    internal interface IInternalEventImpl<TImpl> : IInternalEvent<TImpl>
        where TImpl : IInternalEventImpl<TImpl>
    {
        ICreatedEvent<TImpl>? Holder { get; set; }
    }

    internal static class InternalEventExtensions
    {
        public static TImpl Reset<TImpl>(this ICreatedEvent<TImpl> @event) where TImpl : struct, IInternalEventImpl<TImpl>
        {
            var copy = @event.GetInternalEvent();
            @event.GetInternalEvent() = default;
            copy.Holder = null;
            return copy;
        }
    }
}
