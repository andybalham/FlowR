using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlowR.StepLibrary.Activities;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class MockedDecisionFlowRequest : FlowActivityRequest<MockedDecisionFlowResponse>
    {
        public int IntValue { get; set; }
    }

    public class MockedDecisionFlowResponse : FlowResponse
    {
        public string BranchValue { get; set; }
    }

    public class MockedDecisionFlow : FlowHandler<MockedDecisionFlowRequest, MockedDecisionFlowResponse>
    {
        public MockedDecisionFlow(IMediator mediator) : base(mediator)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            var intValue = FlowValueDecision<int?>.NewDefinition()
                .BindInput(rq => rq.SwitchValue, nameof(MockedDecisionFlowRequest.IntValue));

            var setOutputToX = new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                .SetValue(rq => rq.OutputValue, "X")
                .BindOutput(rs => rs.Output, nameof(MockedDecisionFlowResponse.BranchValue));

            var setOutputToY = new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                .SetValue(rq => rq.OutputValue, "Y")
                .BindOutput(rs => rs.Output, nameof(MockedDecisionFlowResponse.BranchValue));

            var setOutputToZ = new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                .SetValue(rq => rq.OutputValue, "Z")
                .BindOutput(rs => rs.Output, nameof(MockedDecisionFlowResponse.BranchValue));

            return new FlowDefinition()
                .Check("Int_value", intValue)
                .When(1).Goto("Set_output_to_X")
                .When(2).Goto("Set_output_to_Y")
                .Else().Goto("Set_output_to_Z")

                .Do("Set_output_to_X", setOutputToX)
                .End()

                .Do("Set_output_to_Y", setOutputToY)
                .End()

                .Do("Set_output_to_Z", setOutputToZ)
                .End();
        }
    }
}
