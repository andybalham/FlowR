using FlowR.StepLibrary.Activities;
using MediatR;

namespace FlowR.Tests.Domain.FlowrBasedMockingTests
{
    public class MockActivityViaFlowRFlowRequest : FlowActivityRequest<MockActivityViaFlowRFlowResponse>
    {
    }

    public class MockActivityViaFlowRFlowResponse : FlowResponse
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
    }

    public class MockActivityViaFlowRFlow : FlowHandler<MockActivityViaFlowRFlowRequest, MockActivityViaFlowRFlowResponse>
    {
        public MockActivityViaFlowRFlow(IMediator mediator, IFlowLogger<MockActivityViaFlowRFlow> logger) : base(mediator, logger)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()
                .Do("Set_value_1_to_A", new FlowOverrideKey("SetValue1"),
                    new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                        .SetValue(r => r.OutputValue, "A")
                        .BindOutput(r => r.Output, "Value1"))
                
                .Do("Set_value_2_to_B", new FlowOverrideKey("SetValue2"),
                    new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                        .SetValue(r => r.OutputValue, "B")
                        .BindOutput(r => r.Output, "Value2"))

                .Do("Set_value_3_to_C", new FlowOverrideKey("SetValue3"),
                    new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                        .SetValue(r => r.OutputValue, "C")
                        .BindOutput(r => r.Output, "Value3"));
        }
    }
}
