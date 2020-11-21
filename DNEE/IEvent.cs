using DNEE.Utility;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DNEE
{
    /// <summary>
    /// A <see langword="dynamic"/> event invocation.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// The <see cref="DataOrigin"/> that the data provided to the handler came from.
        /// </summary>
        DataOrigin DataOrigin { get; }
        /// <summary>
        /// The <see cref="EventName"/> of the event that was invoked.
        /// </summary>
        EventName EventName { get; }
        /// <summary>
        /// Sets the result of this invocation, if any.
        /// </summary>
        dynamic? Result { get; set; }

        /// <summary>
        /// Gets the data passed into this handler invocation.
        /// </summary>
        dynamic? Data { get; }

        /// <summary>
        /// Whether or not <see cref="Next(dynamic?)"/> must always be invoked, either by the event
        /// handler, or by the event system.
        /// </summary>
        /// <remarks>
        /// Set this to <see langword="false"/> to prevent the next handler from being called.
        /// </remarks>
        bool AlwaysInvokeNext { get; set; }

        /// <summary>
        /// Gets the full history of data passed into all handlers.
        /// </summary>
        /// <remarks>
        /// The first element of this <see cref="IEnumerable{T}"/> will always be the current invocation.
        /// The following elements will walk up the invocation stack, with the last element being the value
        /// given at the original invocation of the event.
        /// </remarks>
        IEnumerable<DataWithOrigin> DataHistory { get; }

        /// <summary>
        /// Invokes the next event handler for this event using the data given to the current invocation.
        /// </summary>
        /// <remarks>
        /// If the target handler throws an exception, then this rethrows that exception.
        /// </remarks>
        /// <returns>An <see cref="EventResult"/> wrapping the result of that event handler.</returns>
        EventResult Next();

        /// <summary>
        /// Invokes the next event handler for this event with the specified data.
        /// </summary>
        /// <remarks>
        /// If the target handler throws an exception, then this rethrows that exception.
        /// </remarks>
        /// <param name="data">The data to pass to the next handler.</param>
        /// <returns>An <see cref="EventResult"/> wrapping the result of that event handler.</returns>
        EventResult Next(dynamic? data);
    }

    /// <summary>
    /// An event invocation that takes a strongly-typed value of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to expect the data to be.</typeparam>
    public interface IEvent<T> : IEvent
    {
        /// <summary>
        /// Gets the data passed into this handler invocation.
        /// </summary>
        /// <remarks>
        /// This will always be set. It is particularly useful if the data passed in is not
        /// of type <typeparamref name="T"/>, in which case that data is accessible through 
        /// this property.
        /// </remarks>
        dynamic? DynamicData { get; }

        /// <summary>
        /// Gets the data passed into this handler invocation, as <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="InvalidCastException">Thrown if the data could not be reasonably converted to <typeparamref name="T"/>.</exception>
        new T Data { get; }

        /// <summary>
        /// Gets the full history of data passed into all handlers, with the ability to access each entry
        /// as a strongly-typed value.
        /// </summary>
        /// <remarks>
        /// The first element of this <see cref="IEnumerable{T}"/> will always be the current invocation.
        /// The following elements will walk up the invocation stack, with the last element being the value
        /// given at the original invocation of the event.
        /// </remarks>
        new IEnumerable<DataWithOrigin<T>> DataHistory { get; }

        /// <summary>
        /// Invokes the next event handler for this event with the specified data.
        /// </summary>
        /// <remarks>
        /// If the target handler throws an exception, then this rethrows that exception.
        /// </remarks>
        /// <param name="data">The data to pass to the next handler.</param>
        /// <returns>An <see cref="EventResult"/> wrapping the result of that event handler.</returns>
        EventResult Next(in T data);

        /// <summary>
        /// Invokes the next event handler for this event with the specified data.
        /// </summary>
        /// <remarks>
        /// If the target handler throws an exception, then this rethrows that exception.
        /// </remarks>
        /// <param name="data">The data to pass to the next handler.</param>
        /// <returns>An <see cref="EventResult"/> wrapping the result of that event handler.</returns>
        EventResult Next(IUsableAs<T> data);
    }

    /// <summary>
    /// An event invocation that takes a strongly-typed value of type <typeparamref name="T"/>,
    /// and returns a strongly-typed value of type <typeparamref name="TRet"/>.
    /// </summary>
    /// <typeparam name="T">The type to expect the data to be.</typeparam>
    /// <typeparam name="TRet">The type that the result will be.</typeparam>
    public interface IEvent<T, TRet> : IEvent<T>
    {
        /// <summary>
        /// Sets the result of this invocation, if any.
        /// </summary>
        new TRet Result { get; set; }

        /// <summary>
        /// Invokes the next event handler for this event using the data given to the current invocation.
        /// </summary>
        /// <remarks>
        /// If the target handler throws an exception, then this rethrows that exception.
        /// </remarks>
        /// <returns>An <see cref="EventResult{T}"/> wrapping the result of that event handler.</returns>
        new EventResult<TRet> Next();

        /// <summary>
        /// Invokes the next event handler for this event with the specified data.
        /// </summary>
        /// <remarks>
        /// If the target handler throws an exception, then this rethrows that exception.
        /// </remarks>
        /// <param name="data">The data to pass to the next handler.</param>
        /// <returns>An <see cref="EventResult{T}"/> wrapping the result of that event handler.</returns>
        new EventResult<TRet> Next(in T data);

        /// <summary>
        /// Invokes the next event handler for this event with the specified data.
        /// </summary>
        /// <remarks>
        /// If the target handler throws an exception, then this rethrows that exception.
        /// </remarks>
        /// <param name="data">The data to pass to the next handler.</param>
        /// <returns>An <see cref="EventResult{T}"/> wrapping the result of that event handler.</returns>
        new EventResult<TRet> Next(IUsableAs<T> data);
    }
}