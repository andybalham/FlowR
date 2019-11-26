using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.DiscoveryTarget
{
    public class InvalidFlowRequest : FlowActivityRequest<InvalidFlowResponse>
    {
    }

    public class InvalidFlowResponse
    {
    }

    public class InvalidFlowHandler : FlowHandler<InvalidFlowRequest, InvalidFlowResponse>
    {
        public InvalidFlowHandler(IMediator mediator) : base(mediator)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()
                .Goto("NonExistentStep");
        }
    }
}
