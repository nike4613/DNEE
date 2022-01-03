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
        public virtual bool IsFull => false;

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
                var result = AddDataNext(data);
                if (result is not null)
                    return result;
                // otherwise, bubble out to an X8AssocDataMap
                return new X8AssocDataMap { Map1 = this, Map2 = new TypedAssocDataMap<T>(data) };
            }
        }

        public bool CanInsertType<T>() => !IsFull || this is IAssocNodeOfType<T>;

        public abstract TypedAssocDataMapBase? AddDataNext<T>(T data);

        IHolder? IHolder.WithValue<T>(T value) => AddDataNext(value);
    }

    internal sealed class TypedAssocDataMap : TypedAssocDataMapBase
    {
        public static readonly TypedAssocDataMap Instance = new();
        private TypedAssocDataMap() { }

        public override TypedAssocDataMapBase? AddDataNext<T>(T data)
            => new TypedAssocDataMap<T>(data);
    }

    internal class TypedAssocDataMap<T1> : TypedAssocDataMapBase, IAssocNodeOfType<T1>
    {
        protected readonly T1 val1;
        T1 IAssocValueNodeOfType<T1>.Value => val1;

        public TypedAssocDataMap(T1 val1) => this.val1 = val1;

        public virtual TypedAssocDataMap<T1> Replace1(T1 val1)
            => new TypedAssocDataMap<T1>(val1);

        IAssocNodeOfType<T1> IAssocNodeOfType<T1>.WithReplaced(T1 value) => Replace1(value);

        public override TypedAssocDataMapBase? AddDataNext<T>(T data)
            => new TypedAssocDataMap<T1, T>(val1, data);
    }

    internal class TypedAssocDataMap<T1, T2> : TypedAssocDataMap<T1>, IAssocNodeOfType<T2>
    {
        protected readonly T2 val2;
        T2 IAssocValueNodeOfType<T2>.Value => val2;

        public TypedAssocDataMap(T1 val1, T2 val2) : base(val1) => this.val2 = val2;
        
        public override TypedAssocDataMap<T1> Replace1(T1 val1)
            => new TypedAssocDataMap<T1, T2>(val1, val2);

        public virtual TypedAssocDataMap<T1, T2> Replace2(T2 val2)
            => new TypedAssocDataMap<T1, T2>(val1, val2);

        IAssocNodeOfType<T2> IAssocNodeOfType<T2>.WithReplaced(T2 value) => Replace2(value);

        public override TypedAssocDataMapBase? AddDataNext<T>(T data)
            => new TypedAssocDataMap<T1, T2, T>(val1, val2, data);
    }

    internal class TypedAssocDataMap<T1, T2, T3> : TypedAssocDataMap<T1, T2>, IAssocNodeOfType<T3>
    {
        protected readonly T3 val3;
        T3 IAssocValueNodeOfType<T3>.Value => val3;

        public TypedAssocDataMap(T1 val1, T2 val2, T3 val3) : base(val1, val2) => this.val3 = val3;

        public override TypedAssocDataMap<T1> Replace1(T1 val1)
            => new TypedAssocDataMap<T1, T2, T3>(val1, val2, val3);

        public override TypedAssocDataMap<T1, T2> Replace2(T2 val2)
            => new TypedAssocDataMap<T1, T2, T3>(val1, val2, val3);

        public virtual TypedAssocDataMap<T1, T2, T3> Replace3(T3 val3)
            => new TypedAssocDataMap<T1, T2, T3>(val1, val2, val3);

        IAssocNodeOfType<T3> IAssocNodeOfType<T3>.WithReplaced(T3 value) => Replace3(value);

        public override TypedAssocDataMapBase? AddDataNext<T>(T data)
            => new TypedAssocDataMap<T1, T2, T3, T>(val1, val2, val3, data);
    }

    internal class TypedAssocDataMap<T1, T2, T3, T4> : TypedAssocDataMap<T1, T2, T3>, IAssocNodeOfType<T4>
    {
        protected readonly T4 val4;
        T4 IAssocValueNodeOfType<T4>.Value => val4;

        public TypedAssocDataMap(T1 val1, T2 val2, T3 val3, T4 val4) : base(val1, val2, val3)
            => this.val4 = val4;

        public override TypedAssocDataMap<T1> Replace1(T1 val1)
            => new TypedAssocDataMap<T1, T2, T3, T4>(val1, val2, val3, val4);

        public override TypedAssocDataMap<T1, T2> Replace2(T2 val2)
            => new TypedAssocDataMap<T1, T2, T3, T4>(val1, val2, val3, val4);

        public override TypedAssocDataMap<T1, T2, T3> Replace3(T3 val3)
            => new TypedAssocDataMap<T1, T2, T3, T4>(val1, val2, val3, val4);

        public virtual TypedAssocDataMap<T1, T2, T3, T4> Replace4(T4 val4)
            => new TypedAssocDataMap<T1, T2, T3, T4>(val1, val2, val3, val4);

        IAssocNodeOfType<T4> IAssocNodeOfType<T4>.WithReplaced(T4 value) => Replace4(value);

        public override TypedAssocDataMapBase? AddDataNext<T>(T data)
            => new TypedAssocDataMap<T1, T2, T3, T4, T>(val1, val2, val3, val4, data);
    }

    internal class TypedAssocDataMap<T1, T2, T3, T4, T5> : TypedAssocDataMap<T1, T2, T3, T4>, IAssocNodeOfType<T5>
    {
        protected readonly T5 val5;
        T5 IAssocValueNodeOfType<T5>.Value => val5;

        public TypedAssocDataMap(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5) : base(val1, val2, val3, val4)
            => this.val5 = val5;

        public override TypedAssocDataMap<T1> Replace1(T1 val1)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5>(val1, val2, val3, val4, val5);

        public override TypedAssocDataMap<T1, T2> Replace2(T2 val2)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5>(val1, val2, val3, val4, val5);

        public override TypedAssocDataMap<T1, T2, T3> Replace3(T3 val3)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5>(val1, val2, val3, val4, val5);

        public override TypedAssocDataMap<T1, T2, T3, T4> Replace4(T4 val4)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5>(val1, val2, val3, val4, val5);

        public virtual TypedAssocDataMap<T1, T2, T3, T4, T5> Replace5(T5 val5)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5>(val1, val2, val3, val4, val5);

        IAssocNodeOfType<T5> IAssocNodeOfType<T5>.WithReplaced(T5 value) => Replace5(value);

        public override TypedAssocDataMapBase? AddDataNext<T>(T data)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T>(val1, val2, val3, val4, val5, data);
    }

    internal class TypedAssocDataMap<T1, T2, T3, T4, T5, T6> : TypedAssocDataMap<T1, T2, T3, T4, T5>, IAssocNodeOfType<T6>
    {
        protected readonly T6 val6;
        T6 IAssocValueNodeOfType<T6>.Value => val6;

        public TypedAssocDataMap(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5, T6 val6) : base(val1, val2, val3, val4, val5)
            => this.val6 = val6;

        public override TypedAssocDataMap<T1> Replace1(T1 val1)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6> (val1, val2, val3, val4, val5, val6);

        public override TypedAssocDataMap<T1, T2> Replace2(T2 val2)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6> (val1, val2, val3, val4, val5, val6);

        public override TypedAssocDataMap<T1, T2, T3> Replace3(T3 val3)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6>(val1, val2, val3, val4, val5, val6);

        public override TypedAssocDataMap<T1, T2, T3, T4> Replace4(T4 val4)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6>(val1, val2, val3, val4, val5, val6);

        public override TypedAssocDataMap<T1, T2, T3, T4, T5> Replace5(T5 val5)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6>(val1, val2, val3, val4, val5, val6);

        public virtual TypedAssocDataMap<T1, T2, T3, T4, T5, T6> Replace6(T6 val6)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6>(val1, val2, val3, val4, val5, val6);

        IAssocNodeOfType<T6> IAssocNodeOfType<T6>.WithReplaced(T6 value) => Replace6(value);

        public override TypedAssocDataMapBase? AddDataNext<T>(T data)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T>(val1, val2, val3, val4, val5, val6, data);
    }

    internal class TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7> : TypedAssocDataMap<T1, T2, T3, T4, T5, T6>, IAssocNodeOfType<T7>
    {
        protected readonly T7 val7;
        T7 IAssocValueNodeOfType<T7>.Value => val7;

        public TypedAssocDataMap(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5, T6 val6, T7 val7) : base(val1, val2, val3, val4, val5, val6)
            => this.val7 = val7;

        public override TypedAssocDataMap<T1> Replace1(T1 val1)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7>(val1, val2, val3, val4, val5, val6, val7);

        public override TypedAssocDataMap<T1, T2> Replace2(T2 val2)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7>(val1, val2, val3, val4, val5, val6, val7);

        public override TypedAssocDataMap<T1, T2, T3> Replace3(T3 val3)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7>(val1, val2, val3, val4, val5, val6, val7);

        public override TypedAssocDataMap<T1, T2, T3, T4> Replace4(T4 val4)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7>(val1, val2, val3, val4, val5, val6, val7);

        public override TypedAssocDataMap<T1, T2, T3, T4, T5> Replace5(T5 val5)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7>(val1, val2, val3, val4, val5, val6, val7);

        public override TypedAssocDataMap<T1, T2, T3, T4, T5, T6> Replace6(T6 val6)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7>(val1, val2, val3, val4, val5, val6, val7);

        public virtual TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7> Replace7(T7 val7)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7>(val1, val2, val3, val4, val5, val6, val7);

        IAssocNodeOfType<T7> IAssocNodeOfType<T7>.WithReplaced(T7 value) => Replace7(value);

        public override TypedAssocDataMapBase? AddDataNext<T>(T data)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7, T>(val1, val2, val3, val4, val5, val6, val7, data);
    }

    internal class TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7, T8> : TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7>, IAssocNodeOfType<T8>
    {
        protected readonly T8 val8;
        T8 IAssocValueNodeOfType<T8>.Value => val8;

        public TypedAssocDataMap(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5, T6 val6, T7 val7, T8 val8) : base(val1, val2, val3, val4, val5, val6, val7)
            => this.val8 = val8;

        public override TypedAssocDataMap<T1> Replace1(T1 val1)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7, T8>(val1, val2, val3, val4, val5, val6, val7, val8);

        public override TypedAssocDataMap<T1, T2> Replace2(T2 val2)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7, T8>(val1, val2, val3, val4, val5, val6, val7, val8);

        public override TypedAssocDataMap<T1, T2, T3> Replace3(T3 val3)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7, T8>(val1, val2, val3, val4, val5, val6, val7, val8);

        public override TypedAssocDataMap<T1, T2, T3, T4> Replace4(T4 val4)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7, T8>(val1, val2, val3, val4, val5, val6, val7, val8);

        public override TypedAssocDataMap<T1, T2, T3, T4, T5> Replace5(T5 val5)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7, T8>(val1, val2, val3, val4, val5, val6, val7, val8);

        public override TypedAssocDataMap<T1, T2, T3, T4, T5, T6> Replace6(T6 val6)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7, T8>(val1, val2, val3, val4, val5, val6, val7, val8);

        public override TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7> Replace7(T7 val7)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7, T8>(val1, val2, val3, val4, val5, val6, val7, val8);

        public virtual TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7, T8> Replace8(T8 val8)
            => new TypedAssocDataMap<T1, T2, T3, T4, T5, T6, T7, T8>(val1, val2, val3, val4, val5, val6, val7, val8);

        IAssocNodeOfType<T8> IAssocNodeOfType<T8>.WithReplaced(T8 value) => Replace8(value);

        public override bool IsFull => true;

        // we're full, return null
        public override TypedAssocDataMapBase? AddDataNext<T>(T data) => null;
    }
}
