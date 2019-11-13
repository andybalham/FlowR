using System.Collections.Generic;

namespace FlowR.Discovery
{
    public class FlowDiagramNode
    {
        public FlowDiagramNodeType NodeType { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public string OverrideKey { get; set; }

        public string OverrideDescription { get; set; }

        public IReadOnlyDictionary<string, string> InputSetters { get; set; } = new Dictionary<string, string>();

        public IReadOnlyDictionary<string, string> InputBindings { get; set; } = new Dictionary<string, string>();

        public IReadOnlyDictionary<string, IList<FlowInputOverride>> InputOverrides { get; set; } = 
            new Dictionary<string, IList<FlowInputOverride>>();

        public IReadOnlyDictionary<string, string> OutputBindings { get; set; } = new Dictionary<string, string>();

        public IList<FlowDiagramLink> Links { get; set; } = new List<FlowDiagramLink>();

        public bool HasInputOverrides => this.InputOverrides?.Count > 0;
    }
}