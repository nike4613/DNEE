using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    internal interface IDataHistoryNode
    {
        DataOrigin Origin { get; }
        dynamic? Data { get; }

        IDataHistoryNode? Next { get; }
    }

    internal interface IDataHistoryNode<T> : IDataHistoryNode
    { 
        bool IsTyped { get; }
        new T Data { get; }
    }
}
