using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DNEE
{
    /// <summary>
    /// A wrapper struct representing a piece of <see langword="dynamic"/> data and an associated <see cref="DataOrigin"/>.
    /// </summary>
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", 
        Justification = "This is a simple wrapper type; it shouldn't be compared.")]
    public struct DataWithOrigin
    {
        /// <summary>
        /// Gets the origin associated with the data.
        /// </summary>
        public DataOrigin Origin { get; }
        /// <summary>
        /// Gets the data wrapped by this struct.
        /// </summary>
        public dynamic? Data { get; }

        /// <summary>
        /// Constructs a <see cref="DataWithOrigin"/> struct with the specified data and origin.
        /// </summary>
        /// <param name="origin">The origin to associate with the data.</param>
        /// <param name="data">The data to wrap.</param>
        public DataWithOrigin(DataOrigin origin, dynamic? data)
            => (Origin, Data) = (origin, (object?)data);

        /// <summary>
        /// Deconstructs this <see cref="DataWithOrigin"/> into <c>(origin, data)</c>.
        /// </summary>
        /// <param name="origin">The origin associated with the data.</param>
        /// <param name="data">The data wrapped by this struct.</param>
        public void Deconstruct(out DataOrigin origin, out dynamic? data)
            => (origin, data) = (Origin, Data);
    }

    /// <summary>
    /// A wrapper struct representing a piece of (possibly) typed data and an associated <see cref="DataOrigin"/>.
    /// </summary>
    /// <typeparam name="T">The type that the data is expected to be.</typeparam>
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types",
        Justification = "This is a simple wrapper type; it shouldn't be compared.")]
    public struct DataWithOrigin<T>
    {
        /// <summary>
        /// Gets the origin associated with the data.
        /// </summary>
        public DataOrigin Origin { get; }

        /// <summary>
        /// Gets whether or not the data is of type <typeparamref name="T"/>.
        /// </summary>
        public bool IsTyped { get; }

        private readonly dynamic? dynData;
        /// <summary>
        /// Gets the wrapped data.
        /// </summary>
        public dynamic? DynamicData => IsTyped ? typedData : dynData;

        private readonly T typedData;
        /// <summary>
        /// Gets the data wrapped by this struct, if it is of type <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="IsTyped"/> is <see langword="false"/></exception>
        public T TypedData => IsTyped ? typedData : throw new InvalidOperationException();

        /// <summary>
        /// Constructs a <see cref="DataWithOrigin"/> struct with the specified data and origin.
        /// </summary>
        /// <param name="origin">The origin to associate with the data.</param>
        /// <param name="data">The data to wrap.</param>
        public DataWithOrigin(DataOrigin origin, dynamic? data)
        {
            Origin = origin;
            if (data is T tval)
            {
                IsTyped = true;
                typedData = tval;
                dynData = null;
            }
            else
            {
                IsTyped = false;
                dynData = data;
                typedData = default!;
            }
        }

        /// <summary>
        /// Constructs a <see cref="DataWithOrigin{T}"/> struct with the specified data and origin.
        /// </summary>
        /// <param name="origin">The origin to associate with the data.</param>
        /// <param name="data">The data to wrap.</param>
        public DataWithOrigin(DataOrigin origin, T data)
            => (Origin, IsTyped, typedData, dynData) = (origin, true, data, null);

        /// <summary>
        /// Deconstructs this <see cref="DataWithOrigin{T}"/> into <c>(origin, data)</c>.
        /// </summary>
        /// <param name="origin">The origin associated with the data.</param>
        /// <param name="data">The data wrapped by this struct.</param>
        public void Deconstruct(out DataOrigin origin, out dynamic? data)
            => (origin, data) = (Origin, DynamicData);
    }
}
