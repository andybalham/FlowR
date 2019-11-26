using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FlowR.Discovery
{
    public class FlowDiagram
    {
        #region Member declarations

        public const string RequestOverrideColor = "red";

        #endregion

        #region Public properties

        public string Criteria { get; }

        public FlowRequestInfo Request { get; }

        public FlowResponseInfo Response { get; }

        public bool IsOverride { get; }

        public IList<FlowDiagramNode> Nodes { get; } = new List<FlowDiagramNode>();

        #endregion

        #region Constructors

        public FlowDiagram(string criteria, FlowRequestInfo requestInfo, FlowResponseInfo responseInfo, bool isOverride)
        {
            Criteria = criteria;
            Request = requestInfo;
            Response = responseInfo;
            IsOverride = isOverride;
        }

        #endregion

        #region Public methods

        public FlowDiagramNode AddNode(FlowDiagramNode node)
        {
            this.Nodes.Add(node);
            return node;
        }

        public string GetDotDiagram()
        {
            var formatBuilder = new StringBuilder();
            var flowDiagramBuilder = new StringBuilder();

            foreach (var node in this.Nodes)
            {
                string nodeFormat;

                switch (node.NodeType)
                {
                    case FlowDiagramNodeType.Start:
                        nodeFormat = $"shape=invhouse,label=\"Start\"";
                        break;
                    case FlowDiagramNodeType.End:
                        nodeFormat = $"style=bold,label=\"End\"";
                        break;
                    case FlowDiagramNodeType.Activity:
                        nodeFormat = $"shape=box,label=\"{node.Text}\"";
                        break;
                    case FlowDiagramNodeType.Event:
                        nodeFormat = $"shape=cds,label=\"{node.Text}\"";
                        break;
                    case FlowDiagramNodeType.Decision:
                        nodeFormat = $"shape=diamond,label=\"{node.Text}\"";
                        break;
                    case FlowDiagramNodeType.Label:
                        nodeFormat = $"shape=underline,label=\"{node.Text}\"";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(node.NodeType), $"Unhandled node type: {node.NodeType}");
                }

                if (node.HasInputOverrides)
                {
                    nodeFormat += $",color={FlowDiagram.RequestOverrideColor}";
                }

                formatBuilder.AppendLine($"    {node.Name} [{nodeFormat}];");

                foreach (var link in node.Links ?? new FlowDiagramLink[] { })
                {
                    if ((link.Criteria?.Any()).GetValueOrDefault())
                    {
                        var criteria = string.Join(" or ", link.Criteria.ToArray());
                        flowDiagramBuilder.AppendLine($"    {node.Name} -> {link.TargetNodeName} [label=\"{criteria}\"];");
                    }
                    else
                    {
                        flowDiagramBuilder.AppendLine($"    {node.Name} -> {link.TargetNodeName};");
                    }
                }
            }

            var notationBuilder = new StringBuilder();

            notationBuilder.AppendLine("digraph Flow {");
            notationBuilder.AppendLine("");
            notationBuilder.Append(formatBuilder);
            notationBuilder.AppendLine("");
            notationBuilder.Append(flowDiagramBuilder);
            notationBuilder.AppendLine("}");

            return notationBuilder.ToString();
        }

        #endregion
    }
}