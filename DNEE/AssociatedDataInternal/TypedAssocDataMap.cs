using DNEE.Utility;
using System;

namespace DNEE.AssociatedDataInternal
{
    internal interface IAssocValueNodeOfType<out T> : IAssocDataMap
    {
        T Value { get; }
    }

    internal interface IAssocNodeOfType<T> : IAssocValueNodeOfType<T>
    {
        IAssocNodeOfType<T> WithReplaced(T value);
    }

    internal abstract class TypedAssocDataMapBase : IAssocDataMap, IHolder
    {
        public IHolder? TryGetData<T, THolder>(ref THolder holder) where THolder : IHolder
        {
            if (this is IAssocValueNodeOfType<T> node)
            {
                return holder.WithValue(node.Value);
            }
            else
            {
                return null;
            }
        }

        public IAssocDataMap WithData<T>(T data, out Maybe<T> prev)
        {
            if (this is IAssocNodeOfType<T> node)
            {
                // TODO: check if this value is equivalent to the existing one, if so just return self
                prev = Maybe.Some(node.Value);
                return node.WithReplaced(data);
            }
            else
            {
                prev = Maybe.None;
                return AddDataNext(data);
            }
        }

        public abstract TypedAssocDataMapBase AddDataNext<T>(T data);

        IHolder? IHolder.WithValue<T>(T value) => AddDataNext(value);
    }

    internal sealed class TypedAssocDataMap0 : TypedAssocDataMapBase
    {
        public static readonly TypedAssocDataMap0 Instance = new();
        private TypedAssocDataMap0() { }

        public override TypedAssocDataMapBase AddDataNext<T>(T data)
            => new TypedAssocDataMap1<T>(data);
    }

    internal class TypedAssocDataMap1<T1> : TypedAssocDataMapBase, IAssocNodeOfType<T1>
    {
        protected readonly T1 val1;
        T1 IAssocValueNodeOfType<T1>.Value => val1;

        public TypedAssocDataMap1(T1 val1) => this.val1 = val1;

        public virtual TypedAssocDataMap1<T1> Replace1(T1 val1) => new(val1);

        IAssocNodeOfType<T1> IAssocNodeOfType<T1>.WithReplaced(T1 value) => Replace1(value);

        public override TypedAssocDataMapBase AddDataNext<T>(T data)
            => new TypedAssocDataMap2<T1, T>(val1, data);
    }

    internal class TypedAssocDataMap2<T1, T2> : TypedAssocDataMap1<T1>, IAssocNodeOfType<T2>
    {
        protected readonly T2 val2;
        T2 IAssocValueNodeOfType<T2>.Value => val2;

        public TypedAssocDataMap2(T1 val1, T2 val2) : base(val1) => this.val2 = val2;
        
        public override TypedAssocDataMap1<T1> Replace1(T1 val1)
            => new TypedAssocDataMap2<T1, T2>(val1, val2);

        public virtual TypedAssocDataMap2<T1, T2> Replace2(T2 val2) => new(val1, val2);

        IAssocNodeOfType<T2> IAssocNodeOfType<T2>.WithReplaced(T2 value) => Replace2(value);

        public override TypedAssocDataMapBase AddDataNext<T>(T data)
            => new TypedAssocDataMap3<T1, T2, T>(val1, val2, data);
    }

    internal class TypedAssocDataMap3<T1, T2, T3> : TypedAssocDataMap2<T1, T2>, IAssocNodeOfType<T3>
    {
        protected readonly T3 val3;
        T3 IAssocValueNodeOfType<T3>.Value => val3;

        public TypedAssocDataMap3(T1 val1, T2 val2, T3 val3) : base(val1, val2) => this.val3 = val3;

        public override TypedAssocDataMap1<T1> Replace1(T1 val1)
            => new TypedAssocDataMap3<T1, T2, T3>(val1, val2, val3);

        public override TypedAssocDataMap2<T1, T2> Replace2(T2 val2)
            => new TypedAssocDataMap3<T1, T2, T3>(val1, val2, val3);

        public virtual TypedAssocDataMap3<T1, T2, T3> Replace3(T3 val3) => new(val1, val2, val3);

        IAssocNodeOfType<T3> IAssocNodeOfType<T3>.WithReplaced(T3 value) => Replace3(value);

        public override TypedAssocDataMapBase AddDataNext<T>(T data)
            => new TypedAssocDataMap4<T1, T2, T3, T>(val1, val2, val3, data);
    }


    internal class TypedAssocDataMap4<T1, T2, T3, T4> : TypedAssocDataMap3<T1, T2, T3>, IAssocNodeOfType<T4>
    {
        protected readonly T4 val4;
        T4 IAssocValueNodeOfType<T4>.Value => val4;

        public TypedAssocDataMap4(T1 val1, T2 val2, T3 val3, T4 val4) : base(val1, val2, val3)
            => this.val4 = val4;

        public override TypedAssocDataMap1<T1> Replace1(T1 val1)
            => new TypedAssocDataMap4<T1, T2, T3, T4>(val1, val2, val3, val4);

        public override TypedAssocDataMap2<T1, T2> Replace2(T2 val2)
            => new TypedAssocDataMap4<T1, T2, T3, T4>(val1, val2, val3, val4);

        public override TypedAssocDataMap3<T1, T2, T3> Replace3(T3 val3)
            => new TypedAssocDataMap4<T1, T2, T3, T4>(val1, val2, val3, val4);

        public virtual TypedAssocDataMap4<T1, T2, T3, T4> Replace4(T4 val4)
            => new(val1, val2, val3, val4);

        IAssocNodeOfType<T4> IAssocNodeOfType<T4>.WithReplaced(T4 value) => Replace4(value);

        public override TypedAssocDataMapBase AddDataNext<T>(T data)
        {
            // this needs to forward to some n-map
            throw new NotImplementedException();
        }
    }
}
