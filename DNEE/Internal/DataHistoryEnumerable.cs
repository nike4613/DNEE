using DNEE.Tuning;
using System.Collections;
using System.Collections.Generic;

namespace DNEE.Internal
{
    // TODO: maybe get rid of one of these?

    internal sealed class DataHistoryEnumerable : IEnumerable<DataWithOrigin>
    {
        private readonly ICreatedEvent first;

        public DataHistoryEnumerable(ICreatedEvent firstNode)
            => first = firstNode;

        public IEnumerator<DataWithOrigin> GetEnumerator()
            => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public sealed class Enumerator : IEnumerator<DataWithOrigin>
        {
            private readonly ICreatedEvent start;
            private ICreatedEvent? node;
            private bool started;
            internal Enumerator(DataHistoryEnumerable enumerable)
                => node = start = enumerable.first;

            public DataWithOrigin Current
            {
                get
                {
                    var node = this.node?.Event();
                    return node != null ? new DataWithOrigin(node.DataOrigin, (object?)node.Data, node.EventName) : default;
                }
            }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (!started)
                    return started = true;
                if (node is null) return false;

                var lastData = (object?)node.Event().Data;
                var lastOrigin = node.Event().DataOrigin;
                while (node != null)
                {
                    node = node.GetLastEvent();
                    if (node == null)
                        break;

                    var data = (object?)node.Event().Data;
                    var origin = node.Event().DataOrigin;
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
        private readonly ICreatedEvent first;

        public DataHistoryEnumerable(ICreatedEvent firstNode)
            => first = firstNode;

        public IEnumerator<DataWithOrigin<T>> GetEnumerator()
            => new Enumerator(this);
        IEnumerator<DataWithOrigin> IEnumerable<DataWithOrigin>.GetEnumerator()
            => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public sealed class Enumerator : IEnumerator<DataWithOrigin<T>>, IEnumerator<DataWithOrigin>
        {
            private readonly ICreatedEvent start;
            private ICreatedEvent? node;
            private bool started;
            internal Enumerator(DataHistoryEnumerable<T> enumerable)
                => node = start = enumerable.first;

            public DataWithOrigin<T> Current
            {
                get
                {
                    if (node == null) return default;
                    if (node.IsEvent<T>(out var typed) && typed.HasTypedData)
                        return new DataWithOrigin<T>(typed.DataOrigin, typed.Data, typed.EventName);
                    var evt = node.Event();
                    return new DataWithOrigin<T>(evt.DataOrigin, (object?)evt.Data, evt.EventName);
                }
            }

            object IEnumerator.Current => Current;

            DataWithOrigin IEnumerator<DataWithOrigin>.Current
            {
                get
                {
                    var node = this.node?.Event();
                    return node != null ? new DataWithOrigin(node.DataOrigin, (object?)node.Data, node.EventName) : default;
                }
            }

            public bool MoveNext()
            {
                if (!started)
                    return started = true;
                if (node is null) return false;

                var lastData = (object?)node.Event().Data;
                var lastOrigin = node.Event().DataOrigin;
                while (node != null)
                {
                    node = node.GetLastEvent();
                    if (node == null)
                        break;

                    var data = (object?)node.Event().Data;
                    var origin = node.Event().DataOrigin;
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
