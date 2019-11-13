using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class EnableableStepsFlowRequest : FlowActivityRequest<EnableableStepsFlowResponse>
    {
    }

    public class EnableableStepsFlowResponse : FlowResponse
    {
    }

    public class EnableableStepsFlow : FlowHandler<EnableableStepsFlowRequest, EnableableStepsFlowResponse>
    {
        public EnableableStepsFlow(IMediator mediator, IFlowLogger<EnableableStepsFlow> flowLogger) : base(mediator, flowLogger)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()

                .Do("Activity_Default",
                    new FlowActivityDefinition<EnableableActivityRequest, EnableableActivityResponse>())
                .Do("Activity_Enabled",
                    new FlowActivityDefinition<EnableableActivityRequest, EnableableActivityResponse>()
                        .SetValue(rq => rq.IsEnabled, true));
        }
    }

    public class EnableableActivityRequest : FlowActivityRequest<EnableableActivityResponse>, IFlowStepEnableable
    {
        public bool IsEnabled { get; set; }
    }

    public class EnableableActivityResponse
    {
    }

    public class EnableableActivity : RequestHandler<EnableableActivityRequest, EnableableActivityResponse>
    {
        protected override EnableableActivityResponse Handle(EnableableActivityRequest request)
        {
            return new EnableableActivityResponse();
        }
    }
}
