using System;
using System.Runtime.CompilerServices;

namespace DNEE
{
    /// <summary>
    /// An <see cref="ITypeConverter"/> which converts from one type to another using a provided delegate, 
    /// accepting any parameter of type <typeparamref name="TIn"/>.
    /// </summary>
    /// <typeparam name="TIn">The type that can be converted from.</typeparam>
    /// <typeparam name="TOut">The type that can be converted to.</typeparam>
    public sealed class FuncConverter<TIn, TOut> : TypeConverter<TOut>, FuncConverter<TIn, TOut>.INormalize<TIn>
    {
        private readonly Func<TIn, TOut> converter;

        /// <summary>
        /// Constructs a <see cref="FuncConverter{TIn, TOut}"/> with the specified conversion function.
        /// </summary>
        /// <param name="convert">The function that this instance will use to convert objects.</param>
        public FuncConverter(Func<TIn, TOut> convert)
        {
            if (convert is null)
                throw new ArgumentNullException(nameof(convert));
            converter = convert;
        }

        // all this fuckery allows me to properly (statically) ensure the argument is the right type and convert
        // and, notably, if it *is* the right type, performs no allocation

        private interface INormalize<in T>
        {
            TIn Normalize(T from);
        }

        private sealed class CastNorm : INormalize<object?>
        {
            public static CastNorm Inst { get; } = new();
            public TIn Normalize(object? from) => (TIn)from!;
        }

        private readonly struct Norm<T> : INormalize<T>
        {
            public INormalize<T> Converter { get; }
            public Norm(object own)
            {
                Converter = (own as INormalize<T>)!;
                Converter ??= (CastNorm.Inst as INormalize<T>)!;
                Converter ??= this;
            }

            TIn INormalize<T>.Normalize(T from) => throw new InvalidCastException();
        }

        TIn INormalize<TIn>.Normalize(TIn from) => from;

        /// <inheritdoc/>
        public override bool CanConvert<TFrom>(TFrom from)
            => from is TIn;

        /// <inheritdoc/>
        public override TOut ConvertTo<TFrom>(TFrom from)
            => converter(new Norm<TFrom>(this).Converter.Normalize(from));
    }
}
