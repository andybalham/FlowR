using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlowR.Tests.Domain.FlowTests
{

    public class ActivityOutputFlowRequest : FlowActivityRequest<ActivityOutputFlowResponse>
    {
    }

    public class ActivityOutputFlowResponse : FlowResponse
    {
        public string OutputValue { get; set; }
    }

    public class ActivityOutputFlow : FlowHandler<ActivityOutputFlowRequest, ActivityOutputFlowResponse>
    {
        public ActivityOutputFlow(IMediator mediator, IFlowLogger<ActivityOutputFlow> logger) : base(mediator, logger)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition<ActivityOutputFlowRequest, ActivityOutputFlowResponse> flowDefinition)
        {
            flowDefinition
                .Do("SingleOutput", new FlowActivityDefinition<SingleOutputActivityRequest, SingleOutputActivityResponse>());
        }
    }

    public class SingleOutputActivityRequest : FlowActivityRequest<SingleOutputActivityResponse>
    {
    }

    public class SingleOutputActivityResponse
    {
        public const string ExpectedOutputValue = "SingleOutputActivityResponse.OverridableOutputValue";

        public string OutputValue => ExpectedOutputValue;
    }

    public class SingleOutputActivity : IRequestHandler<SingleOutputActivityRequest, SingleOutputActivityResponse>
    {
        public Task<SingleOutputActivityResponse> Handle(SingleOutputActivityRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SingleOutputActivityResponse());
        }
    }
}
