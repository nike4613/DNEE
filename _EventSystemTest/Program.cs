using DNEE;
using DNEE.Utility;
using System;

namespace _EventSystemTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = new EventSource("EventSystemTest");

            var ev1 = source.Event("Event1");
            var ev2 = source.Event("Event2");

            using var h1 = source.SubscribeTo(ev1, (@event, data) =>
            {
                Console.WriteLine($"(h1) {ev1} invoked with {@event} and {data}");
                var ret = @event.Next(data);
                Console.WriteLine($"(h1) Next returned {ret}");
            }, (HandlerPriority)1);

            using var h2 = source.SubscribeTo(ev2, (@event, data) =>
            {
                Console.WriteLine($"(h2) {ev2} invoked with {@event} and {data}");
                //var ret = @event.Next(data);
                @event.NextAndTryTransform((object?)data, a => a);
            }, (HandlerPriority)1);

            using var h3 = source.SubscribeTo(ev1, (@event, data) =>
            {
                Console.WriteLine($"(h3) {ev1} invoked with {@event} and {data}");
                @event.Result = ev1;
            }, (HandlerPriority)0);

            using var h4 = source.SubscribeTo(ev2, (@event, data) =>
            {
                Console.WriteLine($"(h4) {ev2} invoked with {@event} and {data}");
                @event.Result = ev2;
            }, (HandlerPriority)0);

            using var h5 = source.SubscribeTo(ev2, (@event, data) =>
            {
                Console.WriteLine($"(h5) {ev2} invoked with {@event} and {data}");
                @event.Result = ev2;
            }, (HandlerPriority)(-1));

            using var h6 = source.SubscribeTo<EventName>(ev1, (@event, data) =>
            {
                if (data.HasValue)
                    Console.WriteLine($"(h6) Data found of type EventName: {data.Value}");
                else
                    Console.WriteLine($"(h6) Data of unknown type: {@event.DynamicData}");

                @event.NextAndTryTransform(data, _ => _);

            }, (HandlerPriority)2);

            using var h7 = source.SubscribeTo<EventName>(ev1, (@event, data) =>
            {
               if (data.HasValue)
                    Console.WriteLine($"(h7) Data found of type EventName: {data.Value}");
                else
                    Console.WriteLine($"(h7) Data of unknown type: {@event.DynamicData}");

                throw new Exception("h7");

                //@event.NextAndTryTransform(data, _ => _);

            }, (HandlerPriority)(-2));

            using var h8 = source.SubscribeTo<EventName>(ev1, (@event, data) =>
            {
                if (data.HasValue)
                    Console.WriteLine($"(h8) Data found of type EventName: {data.Value}");
                else
                    Console.WriteLine($"(h8) Data of unknown type: {@event.DynamicData}");

                @event.NextAndTryTransform(data, _ => _);

                foreach (var (origin, ldata) in @event.DataHistory)
                {
                    Console.WriteLine($"({origin}) {ldata}");
                }

                throw new Exception("h8");

            }, (HandlerPriority)(-4));

            using var h9 = source.SubscribeTo<EventName>(ev1, (@event, data) =>
            {
                @event.NextAndTryTransform(data, _ => _);
            }, (HandlerPriority)(-3));

            try
            {
                source.SendEventDynamic(ev1, ev2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine();
            source.SendEventDynamic(ev2, ev1);

            Console.WriteLine();

            source.SetBase(ev2, ev1);

            Console.WriteLine();
            try 
            { 
                source.SendEventDynamic(ev2, ev2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            Console.WriteLine();

            var ev3 = source.Event("Event3");

            using var h10 = source.SubscribeTo<int>(ev3, (@event, data) =>
            {
                if (data.HasValue)
                    Console.WriteLine($"(h10) Data is an int ({data.Value})");
                else
                    Console.WriteLine("(h10) Data is not an int");
            }, (HandlerPriority)100);

            source.SendEvent(ev3, ev1);
            source.SendEvent(ev3, 5);

            Console.WriteLine();

            source.AddConverter(new FuncConverter<EventName, int>(e => e.ToString().Length));

            source.SendEvent(ev3, ev1);
            source.SendEvent(ev3, 5);

            Console.WriteLine();
            Console.WriteLine();

            source.Unsubscribe(h1);
            source.Unsubscribe(h2);
        }
    }
}
