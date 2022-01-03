using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.AssociatedDataInternal
{
    internal sealed record X8AssocDataMap : IAssocDataMap
    {
        // we can only hold as many as are available in a TypedAssocDataMap
        public IAssocDataMap? Map1 { get; init; }
        public IAssocDataMap? Map2 { get; init; }
        public IAssocDataMap? Map3 { get; init; }
        public IAssocDataMap? Map4 { get; init; }
        public IAssocDataMap? Map5 { get; init; }
        public IAssocDataMap? Map6 { get; init; }
        public IAssocDataMap? Map7 { get; init; }
        public IAssocDataMap? Map8 { get; init; }

        public bool IsFull
            => Map1 is not null && Map1.IsFull
            && Map2 is not null && Map2.IsFull
            && Map3 is not null && Map3.IsFull
            && Map4 is not null && Map4.IsFull
            && Map5 is not null && Map5.IsFull
            && Map6 is not null && Map6.IsFull
            && Map7 is not null && Map7.IsFull
            && Map8 is not null && Map8.IsFull;

        public bool CanInsertType<T>()
            => (Map1?.CanInsertType<T>() ?? true)
            || (Map2?.CanInsertType<T>() ?? true)
            || (Map3?.CanInsertType<T>() ?? true)
            || (Map4?.CanInsertType<T>() ?? true)
            || (Map5?.CanInsertType<T>() ?? true)
            || (Map6?.CanInsertType<T>() ?? true)
            || (Map7?.CanInsertType<T>() ?? true)
            || (Map8?.CanInsertType<T>() ?? true);

        public IHolder? TryGetData<T, THolder>(ref THolder holder) where THolder : IHolder
        {
            IHolder innerHolder = TypedAssocDataMap.Instance;
            // in my case, I know that this will never return null
            if (Map1 is not null)
                innerHolder = Map1.TryGetData<T, IHolder>(ref innerHolder) ?? innerHolder;
            if (Map2 is not null)
                innerHolder = Map2.TryGetData<T, IHolder>(ref innerHolder) ?? innerHolder;
            if (Map3 is not null)
                innerHolder = Map3.TryGetData<T, IHolder>(ref innerHolder) ?? innerHolder;
            if (Map4 is not null)
                innerHolder = Map4.TryGetData<T, IHolder>(ref innerHolder) ?? innerHolder;
            if (Map5 is not null)
                innerHolder = Map5.TryGetData<T, IHolder>(ref innerHolder) ?? innerHolder;
            if (Map6 is not null)
                innerHolder = Map6.TryGetData<T, IHolder>(ref innerHolder) ?? innerHolder;
            if (Map7 is not null)
                innerHolder = Map7.TryGetData<T, IHolder>(ref innerHolder) ?? innerHolder;
            if (Map8 is not null)
                innerHolder = Map8.TryGetData<T, IHolder>(ref innerHolder) ?? innerHolder;

            return ((IAssocDataMap)innerHolder).TryGetData<T, THolder>(ref holder);
        }

        public IAssocDataMap WithData<T>(T data, out Maybe<T> prev)
        {
            prev = Maybe.None;

            if (Map1 is not null)
            {
                if (Map1.CanInsertType<T>())
                    return this with { Map1 = Map1.WithData(data, out prev) };
            }
            else return this with { Map1 = new TypedAssocDataMap<T>(data) };

            if (Map2 is not null)
            {
                if (Map2.CanInsertType<T>())
                    return this with { Map2 = Map2.WithData(data, out prev) };
            }
            else return this with { Map2 = new TypedAssocDataMap<T>(data) };

            if (Map3 is not null)
            {
                if (Map3.CanInsertType<T>())
                    return this with { Map3 = Map3.WithData(data, out prev) };
            }
            else return this with { Map3 = new TypedAssocDataMap<T>(data) };

            if (Map4 is not null)
            {
                if (Map4.CanInsertType<T>())
                    return this with { Map4 = Map4.WithData(data, out prev) };
            }
            else return this with { Map4 = new TypedAssocDataMap<T>(data) };

            if (Map5 is not null)
            {
                if (Map5.CanInsertType<T>())
                    return this with { Map5 = Map5.WithData(data, out prev) };
            }
            else return this with { Map5 = new TypedAssocDataMap<T>(data) };

            if (Map6 is not null)
            {
                if (Map6.CanInsertType<T>())
                    return this with { Map6 = Map6.WithData(data, out prev) };
            }
            else return this with { Map6 = new TypedAssocDataMap<T>(data) };

            if (Map7 is not null)
            {
                if (Map7.CanInsertType<T>())
                    return this with { Map7 = Map7.WithData(data, out prev) };
            }
            else return this with { Map7 = new TypedAssocDataMap<T>(data) };

            if (Map8 is not null)
            {
                if (Map8.CanInsertType<T>())
                    return this with { Map8 = Map8.WithData(data, out prev) };
            }
            else return this with { Map8 = new TypedAssocDataMap<T>(data) };

            // if nothing took it, lets just auto expand the first
            // TODO: try to make this balanced if possible
            return this with { Map1 = Map1.WithData(data, out prev) };
        }
    }
}
