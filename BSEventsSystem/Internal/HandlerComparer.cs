using System;
using System.Collections.Generic;
using System.Text;

namespace BSEventsSystem.Internal
{
    internal sealed class HandlerComparer : IComparer<IHandler>
    {
        public static readonly HandlerComparer Instance = new();
        public int Compare(IHandler x, IHandler y)
            => ((long)x.Priority).CompareTo((long)y.Priority);
    }
}
