using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    // TODO: maybe get rid of one of these?

    internal sealed class DataHistoryEnumerable : IEnumerable<DataWithOrigin>
    {
        private readonly IDataHistoryNode first;

        public DataHistoryEnumerable(IDataHistoryNode firstNode)
            => first = firstNode;

        public IEnumerator<DataWithOrigin> GetEnumerator()
            => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public sealed class Enumerator : IEnumerator<DataWithOrigin>
        {
            private readonly IDataHistoryNode start;
            private IDataHistoryNode? node;
            private bool started = false;
            internal Enumerator(DataHistoryEnumerable enumerable)
                => node = start = enumerable.first;

            public DataWithOrigin Current => node != null ? new DataWithOrigin(node.Origin, (object?)node.Data) : default;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (!started)
                    return started = true;
                if (node is null) return false;

                var lastData = (object?)node.Data;
                var lastOrigin = node.Origin;
                while (node != null)
                {
                    node = node.Next;
                    if (node == null)
                        break;

                    var data = (object?)node.Data;
                    var origin = node.Origin;
                    if (lastData != data || lastOrigin != origin)
                        break;
                }

                return node != null;
            }

            public void Reset()
                => (node, started) = (start, false);
            public void Dispose() { }
        }
    }

    internal sealed class DataHistoryEnumerable<T> : IEnumerable<DataWithOrigin<T>>, IEnumerable<DataWithOrigin>
    {
        private readonly IDataHistoryNode first;

        public DataHistoryEnumerable(IDataHistoryNode firstNode)
            => first = firstNode;

        public IEnumerator<DataWithOrigin<T>> GetEnumerator()
            => new Enumerator(this);
        IEnumerator<DataWithOrigin> IEnumerable<DataWithOrigin>.GetEnumerator()
            => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public sealed class Enumerator : IEnumerator<DataWithOrigin<T>>, IEnumerator<DataWithOrigin>
        {
            private readonly IDataHistoryNode start;
            private IDataHistoryNode? node;
            private bool started = false;
            internal Enumerator(DataHistoryEnumerable<T> enumerable)
                => node = start = enumerable.first;

            public DataWithOrigin<T> Current
            {
                get
                {
                    if (node == null) return default;
                    if (node is IDataHistoryNode<T> typed && typed.IsTyped)
                        return new DataWithOrigin<T>(typed.Origin, typed.Data);
                    return new DataWithOrigin<T>(node.Origin, (object?)node.Data);
                }
            }

            object IEnumerator.Current => Current;

            DataWithOrigin IEnumerator<DataWithOrigin>.Current => node != null ? new DataWithOrigin(node.Origin, (object?)node.Data) : default;

            public bool MoveNext()
            {
                if (!started)
                    return started = true;
                if (node is null) return false;

                var lastData = (object?)node.Data;
                var lastOrigin = node.Origin;
                while (node != null)
                {
                    node = node.Next;
                    if (node == null)
                        break;

                    var data = (object?)node.Data;
                    var origin = node.Origin;
                    if (lastData != data || lastOrigin != origin)
                        break;
                }

                return node != null;
            }

            public void Reset()
                => (node, started) = (start, false);
            public void Dispose() { }
        }
    }
}
