﻿using DNEE;
using DNEE.Utility;
using System;

namespace _EventSystemTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new EventManager();
            var origin = new DataOriginOwner("EventSystemTest");
            var source = new EventSource(manager, origin);

            var ev1 = source.Event("Event1");
            var ev2 = source.Event("Event2");

            using var h1 = manager.SubscribeTo(origin, ev1, (@event, data) =>
            {
                Console.WriteLine($"(h1) {ev1} invoked with {@event} and {data}");
                var ret = @event.Next(data);
                Console.WriteLine($"(h1) Next returned {ret}");
            }, (HandlerPriority)1);

            using var h2 = manager.SubscribeTo(origin, ev2, (@event, data) =>
            {
                Console.WriteLine($"(h2) {ev2} invoked with {@event} and {data}");
                //var ret = @event.Next(data);
                @event.NextAndTryTransform((object?)data, a => a);
            }, (HandlerPriority)1);

            using var h3 = manager.SubscribeTo(origin, ev1, (@event, data) =>
            {
                Console.WriteLine($"(h3) {ev1} invoked with {@event} and {data}");
                @event.Result = ev1;
            }, (HandlerPriority)0);

            using var h4 = manager.SubscribeTo(origin, ev2, (@event, data) =>
            {
                Console.WriteLine($"(h4) {ev2} invoked with {@event} and {data}");
                @event.Result = ev2;
            }, (HandlerPriority)0);

            using var h5 = manager.SubscribeTo(origin, ev2, (@event, data) =>
            {
                Console.WriteLine($"(h5) {ev2} invoked with {@event} and {data}");
                @event.Result = ev2;
            }, (HandlerPriority)(-1));

            using var h6 = manager.SubscribeTo<EventName>(origin, ev1, (@event, data) =>
            {
                if (data.HasValue)
                    Console.WriteLine($"(h6) Data found of type EventName: {data.Value}");
                else
                    Console.WriteLine($"(h6) Data of unknown type: {@event.DynamicData}");

                @event.NextAndTryTransform(data, _ => _);

            }, (HandlerPriority)2);

            using var h7 = manager.SubscribeTo<EventName>(origin, ev1, (@event, data) =>
            {
               if (data.HasValue)
                    Console.WriteLine($"(h7) Data found of type EventName: {data.Value}");
                else
                    Console.WriteLine($"(h7) Data of unknown type: {@event.DynamicData}");

                throw new Exception("h7");

                //@event.NextAndTryTransform(data, _ => _);

            }, (HandlerPriority)(-2));

            using var h8 = manager.SubscribeTo<EventName>(origin, ev1, (@event, data) =>
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

            using var h9 = manager.SubscribeTo<EventName>(origin, ev1, (@event, data) =>
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

            manager.Unsubscribe(origin, h1);
            manager.Unsubscribe(origin, h2);
        }
    }
}
