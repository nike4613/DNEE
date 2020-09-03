using BSEventsSystem;
using BSEventsSystem.Utility;
using System;

namespace _EventSystemTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var ev1 = EventName.InSelf("Event1");
            var ev2 = EventName.InSelf("Event2");

            var h1 = EventManager.RegisterHandler(ev1, (@event, data) =>
            {
                Console.WriteLine($"{ev1} invoked with {@event} and {data}");
                var ret = @event.Next(data);
                Console.WriteLine($"Next returned {ret}");
                return true;
            }, default);
            var h2 = EventManager.RegisterHandler(ev2, (@event, data) =>
            {
                Console.WriteLine($"{ev2} invoked with {@event} and {data}");
                var ret = @event.Next(data);
                Console.WriteLine($"Next returned {ret}");
                return true;
            }, default);

            EventManager.SendEventDynamic(ev1, ev2);
            EventManager.SendEventDynamic(ev2, ev1);

            EventManager.UnregisterHandler(h1);
            EventManager.UnregisterHandler(h2);
        }
    }
}
