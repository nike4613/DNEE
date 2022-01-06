using System.Diagnostics.CodeAnalysis;

namespace DNEE.Utility
{
    /// <summary>
    /// A set of helpers used by DNEE.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Attempts to coerce an object into a particular type.
        /// </summary>
        /// <remarks>
        /// The coersion tries the following conversions, in order:
        /// <list type="number">
        /// <item>
        /// <description>It attemps a standard object cast; then</description>
        /// </item>
        /// <item>
        /// <description>it checks if the object implements <see cref="IUsableAs{T}"/> and uses that as the result; then</description>
        /// </item>
        /// <item>
        /// <description>it checks if the object implements <see cref="IDynamicallyUsableAs"/> and calls <see cref="IDynamicallyUsableAs.TryAsType{T}(out T)"/>
        /// to attempt a conversion.</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <typeparam name="T">The type to coerce to.</typeparam>
        /// <param name="obj">The object to coerce.</param>
        /// <param name="value">The result of the type coersion.</param>
        /// <returns><see langword="true"/> if the coersion succeeded, <see langword="false"/> otherwise.</returns>
        public static bool TryUseAs<T>(this object? obj, [MaybeNullWhen(false)] out T value)
        {
            if (obj is T asT)
            {
                value = asT;
                return true;
            }
            else if (obj is IUsableAs<T> usable)
            {
                value = usable.AsType;
                return true;
            }
            else if (obj is IDynamicallyUsableAs dyn && dyn.TryAsType<T>(out value))
            {
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }
}
