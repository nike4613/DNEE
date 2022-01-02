using DNEE.Utility;
using System.Diagnostics.CodeAnalysis;
using DNEE.AssociatedDataInternal;

namespace DNEE
{
    public sealed class AssociatedData : IDynamicallyUsableAs
    {
        private IAssocDataMap? assocDataMap;

        /// <summary>
        /// Adds an object of some type to this <see cref="AssociatedData"/> object, returning the previously
        /// existing value, if any.
        /// </summary>
        /// <typeparam name="T">The type of the value to add.</typeparam>
        /// <param name="value">The object to add.</param>
        /// <returns>The value of that type which was already present, if any.</returns>
        public Maybe<T> AddData<T>(T value)
        {
            if (assocDataMap is null)
            {
                assocDataMap = new TypedAssocDataMap1<T>(value);
                return Maybe.None;
            }
            else
            {
                assocDataMap = assocDataMap.WithData(value, out var prev);
                return prev;
            }
        }

        public bool TryGetData<T>([MaybeNullWhen(false)] out T value)
        {
            if (assocDataMap is null)
            {
                value = default;
                return false;
            }
            else
            {
                return assocDataMap.TryGetData(out value);
            }
        }

        bool IDynamicallyUsableAs.TryAsType<T>([MaybeNullWhen(false)] out T value) => TryGetData(out value);
    }
}
