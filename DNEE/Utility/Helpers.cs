using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DNEE.Utility
{
    public static class Helpers
    {
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
