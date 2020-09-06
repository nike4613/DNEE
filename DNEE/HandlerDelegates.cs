using DNEE.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE
{
    public delegate void DynamicEventHandler(IEvent @event, dynamic? data);
    public delegate void NoReturnEventHandler<T>(IEvent<T> @event, Maybe<T> data);
    public delegate void ReturnEventHandler<T, R>(IEvent<T, R> @event, Maybe<T> data);
}
