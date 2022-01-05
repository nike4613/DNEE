﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Tuning
{
    public interface ICreatedEvent
    {
        /// <summary>
        /// Implementers must forward this to <see cref="IInternalEvent{TSelf}.InterfaceContract"/>.
        /// </summary>
        object InterfaceContract { get; }
    }

    public interface ICreatedEvent<TEvent> : ICreatedEvent where TEvent : IInternalEvent<TEvent>
    {
        ref TEvent GetInternalEvent();
    }
}
