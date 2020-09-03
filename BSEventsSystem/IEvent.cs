using BSEventsSystem.Utility;

namespace BSEventsSystem
{
    public interface IEvent
    {
        EventName EventName { get; }
        dynamic Result { set; }

        Maybe<dynamic> Next(dynamic data);
    }

    public interface IEvent<T> : IEvent
    {
        Maybe<dynamic> Next(in T data);
    }

    public interface IEvent<T, R> : IEvent<T>
    {
        new R Result { set; }

        new Maybe<R> Next(in T data);
    }
}