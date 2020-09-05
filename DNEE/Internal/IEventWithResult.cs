using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Internal
{
    internal interface IEventWithResult
    {
        EventResult GetEventResult();

        bool DidCallNext { get; }
    }

    internal interface IEventWithResult<T> : IEventWithResult
    {
        new EventResult<T> GetEventResult();
    }
}
