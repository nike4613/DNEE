using System;

namespace DNEE.Tuning
{
    public struct AllocationHandle<T> : IDisposable
    {
        public T Object { get; }

        private readonly Action<T>? cleanup;

        public AllocationHandle(T @object, Action<T>? cleanup)
        {
            Object = @object;
            this.cleanup = cleanup;
        }

        public void Dispose() => cleanup?.Invoke(Object);
    }
}
