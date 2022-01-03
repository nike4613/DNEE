using DNEE.Utility;

namespace DNEE.AssociatedDataInternal
{
    internal interface IAssocDataMap
    {
        bool IsFull { get; }
        IAssocDataMap WithData<T>(T data, out Maybe<T> prev);
        IHolder? TryGetData<T, THolder>(ref THolder holder) where THolder : IHolder;
        bool CanInsertType<T>();
    }

    internal interface IHolder
    {
        IHolder? WithValue<T>(T value);
    }
}
