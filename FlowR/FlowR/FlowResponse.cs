using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR
{
    public abstract class FlowResponse
    {
        public string CorrelationId { get; internal set; }

        public string RequestId { get; internal set; }

        public string FlowInstanceId { get; internal set; }

        public FlowTrace Trace { get; internal set; }
    }
}
