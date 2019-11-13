using System.Collections.Generic;

namespace FlowR.Discovery
{
    public class FlowDiagramLink
    {
        public string TargetNodeName { get; set; }

        public IEnumerable<string> Criteria { get; set; }
    }
}