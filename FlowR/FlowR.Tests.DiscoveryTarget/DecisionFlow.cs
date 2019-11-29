using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.DiscoveryTarget
{
    public class DecisionFlowRequest : FlowActivityRequest<DecisionFlowResponse>
    {
    }

    public class DecisionFlowResponse : FlowResponse
    {
    }

    public class DecisionFlow : FlowHandler<DecisionFlowRequest, DecisionFlowResponse>
    {
        public DecisionFlow(IMediator mediator) : base(mediator)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
            flowDefinition
                .Label("Label_1")

                .Check("Decision_1", new FlowDecisionDefinition<BasicDecisionRequest, string>())
                .When("X").End()
                .Else().Continue()

                .Check("Decision_2", new FlowDecisionDefinition<BasicDecisionRequest, string>())
                .When("Y", "Z").Goto("Label_1")
                .Else().End();
        }
    }
}
