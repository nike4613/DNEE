using System;
using System.Collections.Generic;
using System.Text;

namespace DNEE
{
    public class HandlerInvocationException : Exception
    {
        public HandlerInvocationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
