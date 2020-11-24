using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Utility
{
    public sealed class LazyEnumerable<T> : IEnumerable<T>
    {
        private Func<IEnumerable<T>> getter;

        public LazyEnumerable(Func<IEnumerable<T>> func)
        {
            if (func is null)
                throw new ArgumentNullException(nameof(func));

            getter = func;
        }

        public IEnumerator<T> GetEnumerator()
            => getter().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
             => GetEnumerator();
    }
}
