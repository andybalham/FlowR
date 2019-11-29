using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR.Tests.Domain.FlowTests
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

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
        }
    }
}
