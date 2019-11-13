using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace FlowR
{
    public class FlowException : Exception
    {
        public FlowException()
        {
        }

        public FlowException(string message) : base(message)
        {
        }

        public FlowException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FlowException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
