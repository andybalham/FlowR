using System;
using System.Collections.Generic;

namespace FlowR.Discovery
{
    public class FlowResponseInfo
    {
        public Type ResponseType { get; set; }
        public string Description { get; set; }
        public IEnumerable<FlowPropertyInfo> Properties { get; set; }
    }
}