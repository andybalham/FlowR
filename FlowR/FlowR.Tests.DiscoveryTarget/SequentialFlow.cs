using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.DiscoveryTarget
{
    public class SequentialFlowRequest : FlowActivityRequest<SequentialFlowResponse>
    {
    }

    public class SequentialFlowResponse : FlowResponse
    {
    }

    public class SequentialFlow : FlowHandler<SequentialFlowRequest, SequentialFlowResponse>
    {
        public SequentialFlow(IMediator mediator) : base(mediator)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
            flowDefinition
                .Label("Label_1")
                .Do("Activity_1", new FlowActivityDefinition<BasicActivityRequest, BasicActivityResponse>())
                .Goto("Label_2")

                .Label("Label_3")
                .Do("Activity_3", new FlowActivityDefinition<BasicActivityRequest, BasicActivityResponse>())
                .End()

                .Label("Label_2")
                .Do("Activity_2", new FlowActivityDefinition<BasicActivityRequest, BasicActivityResponse>())
                .Goto("Label_3")
                ;
        }
    }
}
