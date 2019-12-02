using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.DiscoveryTarget
{
    [Description("Text test request description")]
    public class TextTestFlowRequest : FlowActivityRequest<TextTestFlowResponse>
    {
        [Description("Text test input value description")]
        public string InputValue { get; set; }
    }

    [Description("Text test response description")]
    public class TextTestFlowResponse : FlowResponse
    {
        [Description("Text test output value description")]
        public string OutputValue { get; set; }
    }

    public class TextTestFlow : FlowHandler<TextTestFlowRequest, TextTestFlowResponse>
    {
        public TextTestFlow(IMediator mediator) : base(mediator)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition<TextTestFlowRequest, TextTestFlowResponse> flowDefinition)
        {
            flowDefinition

                .Do("Activity_1", 
                    new FlowOverrideKey("Activity_1-OverrideKey", "Activity_1 override description"),
                    new FlowActivityDefinition<BasicActivityRequest, BasicActivityResponse>())
                .Do("Activity_2",
                    new FlowActivityDefinition<TextTestActivityRequest, TextTestActivityResponse>()
                        .SetValue(r => r.SetValue, "SetValue"))
                .Do("Activity_3", "Custom text",
                    new FlowActivityDefinition<TextTestActivityRequest, TextTestActivityResponse>()
                        .SetValue(r => r.SetValue, "SetValue"))

                .Check("Decision_1", 
                    new FlowOverrideKey("Decision_1-OverrideKey", "Decision_1 override description"),
                    new FlowDecisionDefinition<BasicDecisionRequest, string>())
                .Else().Continue()

                .Check("Decision_2",
                    new FlowDecisionDefinition<TextTestDecisionRequest, string>()
                        .SetValue(r => r.SetValue, "SetValue"))
                .Else().Continue()
                
                .Check("Decision_3", "Custom text",
                    new FlowDecisionDefinition<TextTestDecisionRequest, string>()
                        .SetValue(r => r.SetValue, "SetValue"))
                .Else().Continue()

                .Label("Label_1")
                .Label("Label_2", "Custom text")
                ;

        }
    }
}
