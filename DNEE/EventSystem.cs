using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE
{
    /// <summary>
    /// A static class with helper functions to make some parts of writing event handlers easier.
    /// </summary>
    public static class EventSystem
    {
        /// <summary>
        /// Invokes <see cref="IEvent.Next(dynamic?)"/> and, if it returns a value, applies <paramref name="transformer"/>
        /// to that value before setting <see cref="IEvent.Result"/>.
        /// </summary>
        /// <param name="event">The current event invocation.</param>
        /// <param name="data">The data to pass to <see cref="IEvent.Next(dynamic?)"/>.</param>
        /// <param name="transformer">The transformation function to apply to the result of <see cref="IEvent.Next(dynamic?)"/>.</param>
        /// <returns><see langword="true"/> if a value was gotten and the transformer was run, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="event"/> is <see langword="null"/>
        /// -OR- if <paramref name="transformer"/> is <see langword="null"/>.</exception>
        public static bool NextAndTryTransform(this IEvent @event, dynamic? data, Func<dynamic?, dynamic?> transformer)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));
            if (transformer is null)
                throw new ArgumentNullException(nameof(transformer));

            var result = @event.Next((object?)data);

            if (result.HasValue)
            {
                @event.Result = transformer((object?)result.Result);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Invokes <see cref="IEvent{T}.Next(in T)"/> and, if it returns a value, applies <paramref name="transformer"/>
        /// to that value before setting <see cref="IEvent.Result"/>.
        /// </summary>
        /// <typeparam name="T">The type of the event data.</typeparam>
        /// <param name="event">The current event invocation.</param>
        /// <param name="data">The data to pass to <see cref="IEvent{T}.Next(in T)"/>, if present.</param>
        /// <param name="transformer">The transformation function to apply to the result of <see cref="IEvent{T}.Next(in T)"/>.</param>
        /// <returns><see langword="true"/> if a value was gotten and the transformer was run, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="event"/> is <see langword="null"/>
        /// -OR- if <paramref name="transformer"/> is <see langword="null"/>.</exception>
        public static bool NextAndTryTransform<T>(this IEvent<T> @event, Maybe<T> data, Func<dynamic?, dynamic?> transformer)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));
            if (transformer is null)
                throw new ArgumentNullException(nameof(transformer));

            var result = data.HasValue
                ? @event.Next(data.Value)
                : @event.Next((object?)@event.DynamicData);

            if (result.HasValue)
            {
                @event.Result = transformer((object?)result.Result);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Invokes <see cref="IEvent{T, R}.Next(in T)"/> and, if it returns a value, applies <paramref name="transformer"/>
        /// to that value before setting <see cref="IEvent{T, R}.Result"/>. If the value is not of type <typeparamref name="TRet"/>,
        /// then <paramref name="dynTransformer"/> is applied instead.
        /// </summary>
        /// <remarks>
        /// If <paramref name="dynTransformer"/> is not specified and <see cref="IEvent{T, R}.Next(in T)"/> returns a <see cref="EventResult"/>
        /// that is not typed, then <see cref="IEvent.Result"/> (the <see langword="dynamic"/> one) on <paramref name="event"/> is set to that
        /// value.
        /// </remarks>
        /// <typeparam name="T">The type of the event data.</typeparam>
        /// <typeparam name="TRet">The type to expect the return value to be.</typeparam>
        /// <param name="event">The current event invocation.</param>
        /// <param name="data">The data to pass to <see cref="IEvent{T, R}.Next(in T)"/>, if present.</param>
        /// <param name="transformer">The transformation function to apply to the result of <see cref="IEvent{T, R}.Next(in T)"/>.</param>
        /// <param name="dynTransformer">The transformation function to apply if the result is not of type <typeparamref name="TRet"/>.</param>
        /// <returns><see langword="true"/> if a value was gotten and the transformer was run, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="event"/> is <see langword="null"/>
        /// -OR- if <paramref name="transformer"/> is <see langword="null"/>.</exception>
        public static bool NextAndTryTransform<T, TRet>(this IEvent<T, TRet> @event, in T data, Func<TRet, TRet> transformer, Func<dynamic?, dynamic?>? dynTransformer = null)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));
            if (transformer is null)
                throw new ArgumentNullException(nameof(transformer));

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
