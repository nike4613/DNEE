using DNEE.Utility;
using System.Diagnostics.CodeAnalysis;
using DNEE.AssociatedDataInternal;

namespace DNEE
{
    public sealed class AssociatedData : IDynamicallyUsableAs
    {
        private IAssocDataMap assocDataMap = TypedAssocDataMap0.Instance;

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
