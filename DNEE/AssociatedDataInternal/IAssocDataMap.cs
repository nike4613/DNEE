using DNEE.Utility;
using System.Diagnostics.CodeAnalysis;

namespace DNEE.AssociatedDataInternal
{
    internal interface IAssocDataMap
    {
        IAssocDataMap WithData<T>(T data, out Maybe<T> prev);
        bool TryGetData<T>([MaybeNullWhen(false)] out T data);
    }
}
