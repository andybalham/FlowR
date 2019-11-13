using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class DisableableStepsFlowRequest : FlowActivityRequest<DisableableStepsFlowResponse>
    {
    }

    public class DisableableStepsFlowResponse : FlowResponse
    {
    }

    public class DisableableStepsFlow : FlowHandler<DisableableStepsFlowRequest, DisableableStepsFlowResponse>
    {
        public DisableableStepsFlow(IMediator mediator, IFlowLogger<DisableableStepsFlow> flowLogger) : base(mediator, flowLogger)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()

                .Do("Activity_Default",
                    new FlowActivityDefinition<DisableableActivityRequest, DisableableActivityResponse>())
                .Do("Activity_Disabled",
                    new FlowActivityDefinition<DisableableActivityRequest, DisableableActivityResponse>()
                        .SetValue(rq => rq.IsDisabled, true));
        }
    }

    public class DisableableActivityRequest : FlowActivityRequest<DisableableActivityResponse>, IFlowStepDisableable
    {
        public bool IsDisabled { get; set; }
    }

    public class DisableableActivityResponse
    {
    }

    public class DisableableActivity : RequestHandler<DisableableActivityRequest, DisableableActivityResponse>
    {
        protected override DisableableActivityResponse Handle(DisableableActivityRequest request)
        {
            return new DisableableActivityResponse();
        }
    }
}
