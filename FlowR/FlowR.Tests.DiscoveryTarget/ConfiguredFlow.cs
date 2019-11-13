using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.DiscoveryTarget
{

    public class ConfiguredFlowRequest : FlowActivityRequest<ConfiguredFlowResponse>
    {
    }

    public class ConfiguredFlowResponse : FlowResponse
    {
    }

    public class ConfiguredFlow : FlowHandler<ConfiguredFlowRequest, ConfiguredFlowResponse>
    {
        public ConfiguredFlow(IMediator mediator) : base(mediator)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()
                .Do("Activity_1",
                    new FlowActivityDefinition<ConfigurableActivityRequest, ConfigurableActivityResponse>()
                        .SetValue(r => r.SetValue, "SetValue")
                        .BindInput(r => r.BoundValue, "FlowValue")
                        .BindOutput(r => r.OutputValue, "FlowValue"))

                .Check("Decision_1",
                    new FlowDecisionDefinition<ConfigurableDecisionRequest, string>()
                        .SetValue(r => r.SetValue, "SetValue")
                        .BindInput(r => r.BoundValue, "FlowValue"))
                .When("X").End()
                .Else().Continue();
        }
    }
}
