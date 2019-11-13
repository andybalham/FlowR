using System;
using System.Collections.Generic;

namespace FlowR.Discovery
{
    public class FlowRequestInfo
    {
        public Type RequestType { get; set; }
        public string Description { get; set; }
        public IEnumerable<FlowPropertyInfo> Properties { get; set; }
    }
}