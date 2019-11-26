using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FlowR.Discovery;
using MediatR;

namespace FlowR
{
    public class FlowDiscoveryRequest : IRequest<FlowDiscoveryResponse>
    {
    }

    public class FlowDiscoveryResponse
    {
        public IEnumerable<FlowDiagram> Flows { get; internal set; }
    }

    public class FlowDiscoveryHandler : RequestHandler<FlowDiscoveryRequest, FlowDiscoveryResponse>
    {
        private readonly IEnumerable<IFlowHandler> _flows;
        private readonly IFlowOverrideProvider _overrideProvider;

        public FlowDiscoveryHandler(IEnumerable<IFlowHandler> flows, IFlowOverrideProvider overrideProvider = null)
        {
            _flows = flows;
            _overrideProvider = overrideProvider;
        }

        protected override FlowDiscoveryResponse Handle(FlowDiscoveryRequest request)
        {
            var flowDiagrams = new List<FlowDiagram>();

            var flowDiagramBuilder = new FlowDiagramBuilder(_overrideProvider);

            foreach (var flow in _flows)
            {
                flowDiagrams.Add(
                    flowDiagramBuilder.BuildDiagram(flow.RequestType, flow.ResponseType, flow.GetFlowDefinition()));

                var flowDefinitionOverrides =
                    _overrideProvider?.GetFlowDefinitionOverrides(flow.RequestType)?.ToList();

                if (flowDefinitionOverrides != null)
                {
                    foreach (var flowDefinition in flowDefinitionOverrides)
                    {
                        flowDiagrams.Add(
                            flowDiagramBuilder.BuildDiagram(
                                flow.RequestType, flow.ResponseType, flowDefinition, isOverride: true));
                    }
                }
            }

            return new FlowDiscoveryResponse { Flows = flowDiagrams };
        }
    }
}
