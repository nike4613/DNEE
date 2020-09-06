﻿using DNEE.Utility;
using System.Collections;
using System.Collections.Generic;

namespace DNEE
{
    public interface IEvent
    {
        DataOrigin DataOrigin { get; }
        EventName EventName { get; }
        dynamic? Result { set; }

        bool AlwaysInvokeNext { get; set; }

        IEnumerable<DataWithOrigin> DataHistory { get; }

        EventResult Next(dynamic? data);
    }

    public interface IEvent<T> : IEvent
    {
        dynamic? DynamicData { get; }
        EventResult Next(in T data);
    }

    public interface IEvent<T, R> : IEvent<T>
    {
        new R Result { set; }

        new IEnumerable<DataWithOrigin<T>> DataHistory { get; }

        new EventResult<R> Next(in T data);
    }
}