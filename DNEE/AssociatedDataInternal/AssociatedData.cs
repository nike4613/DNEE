using DNEE.Utility;
using System.Diagnostics.CodeAnalysis;
using DNEE.AssociatedDataInternal;

namespace DNEE
{
    /// <summary>
    /// An object which can have objects of arbitrary types associated with it. It can also be used as any attached type in the event system.
    /// </summary>
    public sealed class AssociatedData : IDynamicallyUsableAs
    {
        private IAssocDataMap assocDataMap = TypedAssocDataMap.Instance;

        /// <summary>
        /// Adds an object of some type to this <see cref="AssociatedData"/> object, returning the previously
        /// existing value, if any.
        /// </summary>
        /// <typeparam name="T">The type of the value to add.</typeparam>
        /// <param name="value">The object to add.</param>
        /// <returns>The value of that type which was already present, if any.</returns>
        public Maybe<T> AddData<T>(T value)
        {
            assocDataMap = assocDataMap.WithData(value, out var prev);
            return prev;
        }

        /// <summary>
        /// Attempts to get data stored for a type.
        /// </summary>
        /// <remarks>
        /// Data is stored covariantly, so if you have a <see cref="string"/> stored and request an 
        /// <see cref="object"/>, it will give you that <see cref="string"/> instance.
        /// </remarks>
        /// <typeparam name="T">The type to try to get.</typeparam>
        /// <param name="value">The gotten value, if any.</param>
        /// <returns><see langword="true"/> if a value was retrieved, <see langword="false"/> otherwise.</returns>
        public bool TryGetData<T>([MaybeNullWhen(false)] out T value)
        {
            SingleValueHolder<T> holder = default;
            _ = assocDataMap.TryGetData<T, SingleValueHolder<T>>(ref holder);
            value = holder.Value;
            return holder.SetValue;
        }

        bool IDynamicallyUsableAs.TryAsType<T>([MaybeNullWhen(false)] out T value) => TryGetData(out value);
    }
}
