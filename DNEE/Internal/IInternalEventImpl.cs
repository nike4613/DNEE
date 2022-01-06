using DNEE.Tuning;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEvent Event(this ICreatedEvent @event) => (IEvent)@event;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEvent<T>(this ICreatedEvent @event, [MaybeNullWhen(false)] out IEvent<T> result)
        {
            if (@event is IEvent<T> ev)
            {
                result = ev;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}
