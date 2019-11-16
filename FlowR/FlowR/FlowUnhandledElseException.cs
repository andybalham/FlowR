using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR
{
    public class FlowUnhandledElseException : Exception
    {
        public FlowUnhandledElseException(string message) : base(message)
        {
        }
    }
}
