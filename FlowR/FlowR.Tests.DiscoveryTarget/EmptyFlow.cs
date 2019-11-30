using System;
using MediatR;

namespace FlowR.Tests.DiscoveryTarget
{
    public class EmptyFlowRequest : FlowActivityRequest<EmptyFlowResponse>
    {
    }

    public class EmptyFlowResponse : FlowResponse
    {
    }

    public class EmptyFlow : FlowHandler<EmptyFlowRequest, EmptyFlowResponse>
    {
        public EmptyFlow(IMediator mediator) : base(mediator)
        {
        }
    }
}
