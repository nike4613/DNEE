using System;
using System.Runtime.CompilerServices;

namespace DNEE
{
    public sealed class FuncConverter<TIn, TOut> : TypeConverter<TOut>, FuncConverter<TIn, TOut>.INormalize<TIn>
    {
        private readonly Func<TIn, TOut> converter;

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

        // if the source type is a reference type, i always want to just cast fo object then to my target type
        // if the source type is a value type, i want to do exactly what i have now

        TIn INormalize<TIn>.Normalize(TIn from) => from;

        public override bool CanConvert<TFrom>(TFrom from)
            => from is TIn;

        public override TOut ConvertTo<TFrom>(TFrom from)
            => converter(new Norm<TFrom>(this).Converter.Normalize(from));
    }
}
