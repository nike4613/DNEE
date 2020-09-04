using BSEventsSystem.Utility;

namespace BSEventsSystem
{
    public interface IEvent
    {
        EventName EventName { get; }
        dynamic? Result { set; }

        bool AlwaysInvokeNext { get; set; }

        EventResult Next(dynamic? data);
    }

    public interface IEvent<T> : IEvent
    {
        dynamic DynamicData { get; }
        EventResult Next(in T data);
    }

    public interface IEvent<T, R> : IEvent<T>
    {
        new R Result { set; }

        new EventResult<R> Next(in T data);
    }
}