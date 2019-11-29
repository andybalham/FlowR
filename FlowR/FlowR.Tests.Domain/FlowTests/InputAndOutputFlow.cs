using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR.Tests.Domain.FlowTests
{
    public class InputAndOutputFlowRequest : FlowActivityRequest<InputAndOutputFlowResponse>
    {
        public int Value { get; set; }
    }

    public class InputAndOutputFlowResponse : FlowResponse
    {
        public int Value { get; set; }
    }

    public class InputAndOutputFlow : FlowHandler<InputAndOutputFlowRequest, InputAndOutputFlowResponse>
    {
        public InputAndOutputFlow(IMediator mediator) : base(mediator)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
        }
    }
}
