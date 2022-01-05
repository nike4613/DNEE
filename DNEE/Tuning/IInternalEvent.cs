using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE.Tuning
{
    public interface IInternalEvent<TSelf> : IEvent where TSelf : IInternalEvent<TSelf>
    {
        ICreatedEvent? LastEvent { get; }
        void SetHolder(ICreatedEvent<TSelf> created);
    }
}
