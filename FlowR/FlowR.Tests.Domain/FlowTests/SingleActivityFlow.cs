using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR.Tests.Domain.FlowTests
{
    public class SingleActivityFlowRequest : FlowActivityRequest<SingleActivityFlowResponse>
    {
    }

    public class SingleActivityFlowResponse : FlowResponse
    {
    }

    public class SingleActivityFlow : FlowHandler<SingleActivityFlowRequest, SingleActivityFlowResponse>
    {
        public SingleActivityFlow(IMediator mediator, IFlowLogger<SingleActivityFlow> logger) : base(mediator, logger)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()
                .Do("DoNothing", new FlowActivityDefinition<DoNothingRequest, DoNothingResponse>());
        }
    }
}
