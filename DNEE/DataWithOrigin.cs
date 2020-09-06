using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE
{
    public struct DataWithOrigin
    {
        public DataOrigin Origin { get; }
        public dynamic? Data { get; }

        public DataWithOrigin(DataOrigin origin, dynamic? data)
            => (Origin, Data) = (origin, (object?)data);

        public void Deconstruct(out DataOrigin origin, out dynamic? data)
            => (origin, data) = (Origin, Data);
    }

    public struct DataWithOrigin<T>
    {
        public DataOrigin Origin { get; }

        public bool IsTyped { get; }

        private readonly dynamic? dynData;
        public dynamic? DynamicData => IsTyped ? typedData : dynData;

        private readonly T typedData;
        public T TypedData => IsTyped ? typedData : throw new InvalidOperationException();

        public DataWithOrigin(DataOrigin origin, dynamic? data)
        {
            Origin = origin;
            if (data is T tval)
            {
                IsTyped = true;
                typedData = tval;
                dynData = null;
            }
            else
            {
                IsTyped = false;
                dynData = data;
                typedData = default!;
            }
        }

        public DataWithOrigin(DataOrigin origin, T data)
            => (Origin, IsTyped, typedData, dynData) = (origin, true, data, null);

        public void Deconstruct(out DataOrigin origin, out dynamic? data)
            => (origin, data) = (Origin, DynamicData);
    }
}
