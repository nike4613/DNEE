using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE
{
    /// <summary>
    /// An event handler that takes a <see langword="dynamic"/> object as its data.
    /// </summary>
    /// <param name="event">The <see cref="IEvent"/> representing this invocation.</param>
    /// <param name="data">The data provided for this invocation.</param>
    public delegate void DynamicEventHandler(IEvent @event, dynamic? data);

    /// <summary>
    /// An event handler that takes a value of type <typeparamref name="T"/>, but does not return
    /// a strongly typed value.
    /// </summary>
    /// <typeparam name="T">The type of the data parameter.</typeparam>
    /// <param name="event">The <see cref="IEvent{T}"/> representing this invocation.</param>
    /// <param name="data">The data provided for this invocation.</param>
    public delegate void NoReturnEventHandler<T>(IEvent<T> @event, Maybe<T> data);

    /// <summary>
    /// An event handler that takes a value of type <typeparamref name="T"/> and returns a strongly
    /// typed value of type <typeparamref name="R"/>.
    /// </summary>
    /// <typeparam name="T">The type of the data parameter.</typeparam>
    /// <typeparam name="R">The type of the result value.</typeparam>
    /// <param name="event">The <see cref="IEvent{T, R}"/> representing this invocation.</param>
    /// <param name="data">The data provided for this invocation.</param>
    public delegate void ReturnEventHandler<T, R>(IEvent<T, R> @event, Maybe<T> data);
}
