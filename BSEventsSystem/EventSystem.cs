using System;
using System.Collections.Generic;
using System.Text;

namespace BSEventsSystem
{
    public static class EventSystem
    {
        public static bool NextAndTryTransform(this IEvent @event, dynamic data, Func<dynamic?, dynamic?> transformer)
        {
            var result = @event.Next((object)data);

            if (result.HasValue)
            {
                @event.Result = transformer((object?)result.Result);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// If <paramref name="dynTransformer"/> is not specified and <see cref="IEvent{T, R}.Next(in T)"/> returns a <see cref="EventResult"/>
        /// that is not typed, then <see cref="IEvent.Result"/> (the <see langword="dynamic"/> one) on <paramref name="event"/> is set to that
        /// value.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="event"></param>
        /// <param name="data"></param>
        /// <param name="transformer"></param>
        /// <param name="dynTransformer"></param>
        /// <returns>Whether or not a value was gotten.</returns>
        public static bool NextAndTryTransform<T, R>(this IEvent<T, R> @event, in T data, Func<R, R> transformer, Func<dynamic?, dynamic?>? dynTransformer = null)
        {
            var result = @event.Next(data);

            if (result.HasValue)
            {
                if (result.IsTyped)
                {
                    @event.Result = transformer(result.Result);
                }
                else
                {
                    if (dynTransformer != null)
                        ((IEvent)@event).Result = dynTransformer((object?)result.DynamicResult);
                    else
                    {
                        ((IEvent)@event).Result = result.DynamicResult;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
