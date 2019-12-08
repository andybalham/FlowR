using System;
using System.Collections.Generic;
using System.Text;
using FlowR;
using MediatR;

namespace ReadmeExampleConsole
{
    public class MyFlowRequest : FlowActivityRequest<MyFlowResponse>
    {
    }

    public class MyFlowResponse
    {
    }

    public class MyFlowHandler : FlowHandler<MyFlowRequest, MyFlowResponse>
    {
        public MyFlowHandler(IMediator mediator) : base(mediator)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition<MyFlowRequest, MyFlowResponse> flowDefinition)
        {
            flowDefinition

                .Do("Activity1", 
                    new FlowActivityDefinition<Activity1Request, Activity1Response>())
                
                .Check("FlowValue", 
                    new FlowDecisionDefinition<FlowValueDecision<string>, string>()
                        .BindInput(req => req.SwitchValue, "FlowValue"))
                .When("2").Goto("Activity2")
                .Else().Goto("Activity3")

                .Do("Activity2", 
                    new FlowActivityDefinition<Activity2Request, Activity2Response>())
                .End()

                .Do("Activity3", 
                    new FlowActivityDefinition<Activity3Request, Activity3Response>());
        }
    }
}
