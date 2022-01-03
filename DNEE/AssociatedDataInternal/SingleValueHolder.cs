namespace DNEE.AssociatedDataInternal
{
    internal struct SingleValueHolder<TValue> : IHolder
    {
        public TValue Value { get; private set; }
        public bool SetValue { get; private set; }

        private interface II<in T>
        {
            TValue To(T value);
        }

        private class Helper : II<TValue>
        {
            public static readonly Helper Instance = new();

            public TValue To(TValue value) => value;
        }

        public IHolder? WithValue<T>(T value)
        {
            if (Helper.Instance is II<T> ii)
            {
                Value = ii.To(value);
                SetValue = true;
            }

            return null;
        }
    }
}
