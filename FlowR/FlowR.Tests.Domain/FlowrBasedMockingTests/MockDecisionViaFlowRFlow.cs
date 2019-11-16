using FlowR.StepLibrary.Activities;
using MediatR;

namespace FlowR.Tests.Domain.FlowrBasedMockingTests
{

    public class MockDecisionViaFlowRFlowRequest : FlowActivityRequest<MockDecisionViaFlowRFlowResponse>
    {
        public bool SwitchValue { get; set; }
    }

    public class MockDecisionViaFlowRFlowResponse : FlowResponse
    {
        public string Output { get; set; }
    }

    public class MockDecisionViaFlowRFlow : FlowHandler<MockDecisionViaFlowRFlowRequest, MockDecisionViaFlowRFlowResponse>
    {
        public MockDecisionViaFlowRFlow(IMediator mediator, IFlowLogger<MockDecisionViaFlowRFlow> logger) : base(mediator, logger)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()
                .Check("Is_switch_true_1", new FlowOverrideKey("Decision1"),
                    FlowValueDecision<bool?>.NewDefinition())
                .When(true).Goto("Is_switch_true_2")
                .Else().Continue()

                .Do("Set_output_to_A",
                    new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                        .SetValue(r => r.OutputValue, "A"))
                .End()

                .Check("Is_switch_true_2", new FlowOverrideKey("Decision2"),
                    FlowValueDecision<bool?>.NewDefinition())
                .When(true).Goto("Set_output_to_C")
                .Else().Continue()

                .Do("Set_output_to_B",
                    new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                        .SetValue(r => r.OutputValue, "B"))
                .End()

                .Do("Set_output_to_C",
                    new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                        .SetValue(r => r.OutputValue, "C"));
        }
    }
}
