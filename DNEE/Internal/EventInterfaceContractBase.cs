using DNEE.Tuning;

namespace DNEE.Internal
{
    internal abstract class EventInterfaceContractBase
    {
        public abstract ICreatedEvent? GetLastEvent(ICreatedEvent @event);
    }

    internal abstract class EventInterfaceContract<T> : EventInterfaceContractBase
        where T : IInternalEvent<T>
    {
        public override sealed ICreatedEvent? GetLastEvent(ICreatedEvent @event)
        {
            var real = (ICreatedEvent<T>)@event;
            return GetLastEventCore(ref real.GetInternalEvent());
        }

        public abstract ICreatedEvent? GetLastEventCore(ref T @event);
    }

    internal static class EventInteraceContractExtensions
    {
        public static ICreatedEvent? GetLastEvent(this ICreatedEvent evt)
            => ((EventInterfaceContractBase)evt.InterfaceContract).GetLastEvent(evt);
    }
}
