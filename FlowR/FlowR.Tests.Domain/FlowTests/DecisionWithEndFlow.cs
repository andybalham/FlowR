using System;
using System.Collections.Generic;
using System.Text;
using FlowR.StepLibrary.Activities;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{

    public class DecisionWithEndFlowRequest : FlowActivityRequest<DecisionWithEndFlowResponse>
    {
        public string StringValue { get; set; }
    }

    public class DecisionWithEndFlowResponse : FlowResponse
    {
        public string BranchValue { get; set; }
    }

    public class DecisionWithEndFlow : FlowHandler<DecisionWithEndFlowRequest, DecisionWithEndFlowResponse>
    {
        public DecisionWithEndFlow(IMediator mediator) : base(mediator)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
            var switchValueDecision = FlowValueDecision<string>.NewDefinition()
                .BindInput(rq => rq.SwitchValue, nameof(DecisionWithEndFlowRequest.StringValue));

            var setOutputToX = new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                .SetValue(rq => rq.OutputValue, "X")
                .BindOutput(rs => rs.Output, nameof(DecisionWithEndFlowResponse.BranchValue));

            var setOutputToY = new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                .SetValue(rq => rq.OutputValue, "Y")
                .BindOutput(rs => rs.Output, nameof(DecisionWithEndFlowResponse.BranchValue));

            var setOutputToZ = new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                .SetValue(rq => rq.OutputValue, "Z")
                .BindOutput(rs => rs.Output, nameof(DecisionWithEndFlowResponse.BranchValue));

            flowDefinition
                .Do("Set_output_to_X", setOutputToX)
                
                .Check("Switch_value_1", switchValueDecision)
                .When("A").End()
                .When("B").Goto("Set_output_to_Y")
                .Else().Continue()

                .Do("Set_output_to_Y", setOutputToY)

                .Check("Switch_value_2", switchValueDecision)
                .When("C").Goto("Set_output_to_Z")
                .Else().End()

                .Do("Set_output_to_Z", setOutputToZ)
                ;
        }
    }
}
