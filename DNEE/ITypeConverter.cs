using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE
{
    public interface ITypeConverter
    {
        bool CanConvertTo<TTo, TFrom>(TFrom from);

        T ConvertTo<T, TFrom>(TFrom from);
    }

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

        public abstract bool CanConvert<TFrom>(TFrom from);

        public abstract T ConvertTo<TFrom>(TFrom from);
    }
}
