using DNEE.Utility;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DNEE
{
    /// <summary>
    /// A wrapper struct representing a piece of <see langword="dynamic"/> data and an associated <see cref="DataOrigin"/>.
    /// </summary>
    public readonly struct DataWithOrigin
    {
        /// <summary>
        /// Gets the origin associated with the data.
        /// </summary>
        public DataOrigin Origin { get; }

        /// <summary>
        /// The event that this data was provided to.
        /// </summary>
        public EventName Event { get; }

        /// <summary>
        /// Gets the data wrapped by this struct.
        /// </summary>
        public dynamic? Data { get; }

        /// <summary>
        /// Constructs a <see cref="DataWithOrigin"/> struct with the specified data and origin.
        /// </summary>
        /// <param name="origin">The origin to associate with the data.</param>
        /// <param name="data">The data to wrap.</param>
        /// <param name="event">The event that this data was provided to.</param>
        public DataWithOrigin(DataOrigin origin, dynamic? data, EventName @event)
            => (Origin, Data, Event) = (origin, (object?)data, @event);

        /// <summary>
        /// Deconstructs this <see cref="DataWithOrigin"/> into <c>(origin, data)</c>.
        /// </summary>
        /// <param name="origin">The origin associated with the data.</param>
        /// <param name="data">The data wrapped by this struct.</param>
        public void Deconstruct(out DataOrigin origin, out dynamic? data)
            => (origin, data) = (Origin, Data);

        /// <summary>
        /// Deconstructs this <see cref="DataWithOrigin"/> into <c>(origin, data, @event)</c>.
        /// </summary>
        /// <param name="origin">The origin associated with the data.</param>
        /// <param name="data">The data wrapped by this struct.</param>
        /// <param name="event">The event that this data was provided to.</param>
        public void Deconstruct(out DataOrigin origin, out dynamic? data, out EventName @event)
            => (origin, data, @event) = (Origin, Data, Event);
    }

    /// <summary>
    /// A wrapper struct representing a piece of (possibly) typed data and an associated <see cref="DataOrigin"/>.
    /// </summary>
    /// <typeparam name="T">The type that the data is expected to be.</typeparam>
    public readonly struct DataWithOrigin<T>
    {
        /// <summary>
        /// Gets the origin associated with the data.
        /// </summary>
        public DataOrigin Origin { get; }

        /// <summary>
        /// The event that this data was provided to.
        /// </summary>
        public EventName Event { get; }

        /// <summary>
        /// Gets whether or not the data is of type <typeparamref name="T"/>.
        /// </summary>
        [MemberNotNullWhen(true, nameof(typedData))]
        public bool IsTyped { get; }

        private readonly dynamic? dynData;
        /// <summary>
        /// Gets the wrapped data.
        /// </summary>
        /// <remarks>
        /// The object returned may be different than the one returned from <see cref="TypedData"/> if <see cref="IsTyped"/> is set.
        /// This can happen when this <see cref="DataWithOrigin{T}"/> was constructed with an object implementing <see cref="IUsableAs{T}"/>.
        /// </remarks>
        public dynamic? DynamicData => IsTyped ? dynData ?? typedData : dynData;

        private readonly T? typedData;
        /// <summary>
        /// Gets the data wrapped by this struct, if it is of type <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="IsTyped"/> is <see langword="false"/></exception>
        public T TypedData => IsTyped ? typedData : throw new InvalidOperationException();

        /// <summary>
        /// Constructs a <see cref="DataWithOrigin{T}"/> struct with the specified data and origin.
        /// </summary>
        /// <param name="origin">The origin to associate with the data.</param>
        /// <param name="data">The data to wrap.</param>
        public DataWithOrigin(DataOrigin origin, dynamic? data)
        {
            Origin = origin;
            Event = default;
            if (Helpers.TryUseAs<T>((object?)data, out var tval))
            {
                IsTyped = true;
                typedData = tval;
                dynData = data;
            }
            else
            {
                IsTyped = false;
                dynData = data;
                typedData = default;
            }
        }

        /// <summary>
        /// Constructs a <see cref="DataWithOrigin{T}"/> struct with the specified data, origin, and event name.
        /// </summary>
        /// <param name="origin">The origin to associate with the data.</param>
        /// <param name="data">The data to wrap.</param>
        /// <param name="event">The event that this data was provided to.</param>
        public DataWithOrigin(DataOrigin origin, dynamic? data, EventName @event) : this(origin, (object?)data)
            => Event = @event;

        /// <summary>
        /// Constructs a <see cref="DataWithOrigin{T}"/> struct with the specified data and origin.
        /// </summary>
        /// <param name="origin">The origin to associate with the data.</param>
        /// <param name="data">The data to wrap.</param>
        public DataWithOrigin(DataOrigin origin, T data)
            => (Origin, IsTyped, typedData, dynData, Event) = (origin, true, data, null, default);


        /// <summary>
        /// Constructs a <see cref="DataWithOrigin{T}"/> struct with the specified data, origin, and event name.
        /// </summary>
        /// <param name="origin">The origin to associate with the data.</param>
        /// <param name="data">The data to wrap.</param>
        /// <param name="event">The event that this data was provided to.</param>
        public DataWithOrigin(DataOrigin origin, T data, EventName @event) : this(origin, data)
            => Event = @event;

        /// <summary>
        /// Deconstructs this <see cref="DataWithOrigin{T}"/> into <c>(origin, data)</c>.
        /// </summary>
        /// <param name="origin">The origin associated with the data.</param>
        /// <param name="data">The data wrapped by this struct.</param>
        public void Deconstruct(out DataOrigin origin, out dynamic? data)
            => (origin, data) = (Origin, DynamicData);

        /// <summary>
        /// Deconstructs this <see cref="DataWithOrigin{T}"/> into <c>(origin, data, @event)</c>.
        /// </summary>
        /// <param name="origin">The origin associated with the data.</param>
        /// <param name="data">The data wrapped by this struct.</param>
        /// <param name="event">The event that this data was provided to.</param>
        public void Deconstruct(out DataOrigin origin, out dynamic? data, out EventName @event)
            => (origin, data, @event) = (Origin, DynamicData, Event);
    }
}
