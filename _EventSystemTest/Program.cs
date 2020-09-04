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

            using var h1 = EventManager.RegisterHandler(ev1, (@event, data) =>
            {
                Console.WriteLine($"(h1) {ev1} invoked with {@event} and {data}");
                var ret = @event.Next(data);
                Console.WriteLine($"(h1) Next returned {ret}");
            }, (HandlerPriority)1);

            using var h2 = EventManager.RegisterHandler(ev2, (@event, data) =>
            {
                Console.WriteLine($"(h2) {ev2} invoked with {@event} and {data}");
                //var ret = @event.Next(data);
                @event.NextAndTryTransform((object?)data, a => a);
            }, (HandlerPriority)1);

            using var h3 = EventManager.RegisterHandler(ev1, (@event, data) =>
            {
                Console.WriteLine($"(h3) {ev1} invoked with {@event} and {data}");
                @event.Result = ev1;
            }, (HandlerPriority)0);

            using var h4 = EventManager.RegisterHandler(ev2, (@event, data) =>
            {
                Console.WriteLine($"(h4) {ev2} invoked with {@event} and {data}");
                @event.Result = ev2;
            }, (HandlerPriority)0);

            using var h5 = EventManager.RegisterHandler(ev2, (@event, data) =>
            {
                Console.WriteLine($"(h5) {ev2} invoked with {@event} and {data}");
                @event.Result = ev2;
            }, (HandlerPriority)(-1));

            using var h6 = EventManager.RegisterHandler<EventName>(ev1, (@event, data) =>
            {
                if (data.HasValue)
                    Console.WriteLine($"(h6) Data found of type EventName: {data.Value}");
                else
                    Console.WriteLine($"(h6) Data of unknown type: {@event.DynamicData}");

                @event.NextAndTryTransform(data, _ => _);

            }, (HandlerPriority)2);

            using var h7 = EventManager.RegisterHandler<EventName>(ev1, (@event, data) =>
            {
                if (data.HasValue)
                    Console.WriteLine($"(h7) Data found of type EventName: {data.Value}");
                else
                    Console.WriteLine($"(h7) Data of unknown type: {@event.DynamicData}");

                @event.NextAndTryTransform(data, _ => _);

            }, (HandlerPriority)(-2));

            EventManager.SendEventDynamic(ev1, ev2);
            Console.WriteLine();
            EventManager.SendEventDynamic(ev2, ev1);

            EventManager.UnregisterHandler(h1);
            EventManager.UnregisterHandler(h2);
        }
    }
}
