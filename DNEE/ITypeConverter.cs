using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE
{
    /// <summary>
    /// An object which defines a to convert data between types (possibly dynamically) for event handlers.
    /// </summary>
    public interface ITypeConverter
    {
        /// <summary>
        /// Tests if this converter can convert <paramref name="from"/> into an instance of <typeparamref name="TTo"/>.
        /// </summary>
        /// <typeparam name="TTo">The type to try to convert to.</typeparam>
        /// <typeparam name="TFrom">The static type being converted from.</typeparam>
        /// <param name="from">The value to convert.</param>
        /// <returns><see langword="true"/> if this converter can convert the value, <see langword="false"/> otherwise.</returns>
        bool CanConvertTo<TTo, TFrom>(TFrom from);

        /// <summary>
        /// Converts the value in <paramref name="from"/> into an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// This should never be called with type arguments or arguments that were not first checked with
        /// <see cref="CanConvertTo{TTo, TFrom}(TFrom)"/>.
        /// </remarks>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <typeparam name="TFrom">The static type being converted from.</typeparam>
        /// <param name="from">The value to convert.</param>
        /// <returns>The converted value.</returns>
        T ConvertTo<T, TFrom>(TFrom from);
    }

    /// <summary>
    /// A <see cref="ITypeConverter"/> which can only convert to type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type that this converter can convert to.</typeparam>
    public abstract class TypeConverter<T> : ITypeConverter, TypeConverter<T>.IConvFrom<T>
    {
        bool ITypeConverter.CanConvertTo<T1, TFrom>(TFrom from)
            => typeof(T1).IsAssignableFrom(typeof(T)) && CanConvert(from);

        T1 ITypeConverter.ConvertTo<T1, TFrom>(TFrom from)
        {
            if (!typeof(T1).IsAssignableFrom(typeof(T)))
                throw new InvalidCastException();

            return new Conv<T1>(this).Converter.ConvertTo(from);
        }

        // all this fuckery allows for everything to be properly typed, with no allocations in the happy path

        private interface IConvFrom<out T2>
        {
            T2 ConvertTo<TFrom>(TFrom from);
        }

        private readonly struct Conv<T2> : IConvFrom<T2>
        {
            public IConvFrom<T2> Converter { get; }
            public Conv(object self)
            {
                Converter = (self as IConvFrom<T2>)!;
                Converter ??= this;
            }
            public T2 ConvertTo<TFrom>(TFrom from)
                => throw new InvalidCastException();
        }

        /// <summary>
        /// Tests if this converter can convert <paramref name="from"/> into an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="TFrom">The static type being converted from.</typeparam>
        /// <param name="from">The value to convert.</param>
        /// <returns><see langword="true"/> if this converter can convert the value, <see langword="false"/> otherwise.</returns>
        public abstract bool CanConvert<TFrom>(TFrom from);

        /// <summary>
        /// Converts the value in <paramref name="from"/> into an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// This should never be called with type arguments or arguments that were not first checked with
        /// <see cref="CanConvert{TFrom}(TFrom)"/>.
        /// </remarks>
        /// <typeparam name="TFrom">The static type being converted from.</typeparam>
        /// <param name="from">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public abstract T ConvertTo<TFrom>(TFrom from);
    }
}
