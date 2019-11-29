using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{

    public class ActivityStaticValueBindingsFlowRequest : FlowActivityRequest<ActivityStaticValueBindingsFlowResponse>
    {
        public string FlowInput { get; set; }
    }

    public class ActivityStaticValueBindingsFlowResponse : FlowResponse
    {
        public string FlowOutput { get; set; }
    }

    public class ActivityStaticValueBindingsFlow : FlowHandler<ActivityStaticValueBindingsFlowRequest, ActivityStaticValueBindingsFlowResponse>
    {
        public ActivityStaticValueBindingsFlow(IMediator mediator, IFlowLogger<ActivityStaticValueBindingsFlow> logger) : base(mediator, logger)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
            flowDefinition
                .Do("BindFlowValues", new FlowActivityDefinition<StaticBindFlowValuesActivityRequest, StaticBindFlowValuesActivityResponse>()
                    .BindInput(rq => rq.ActivityInputValue, nameof(ActivityStaticValueBindingsFlowRequest.FlowInput))
                    .BindOutput(rs => rs.ActivityOutputValue, nameof(ActivityStaticValueBindingsFlowResponse.FlowOutput)));
        }
    }


    public class StaticBindFlowValuesActivityRequest : FlowActivityRequest<StaticBindFlowValuesActivityResponse>
    {
        public string ActivityInputValue { get; set; }
    }

    public class StaticBindFlowValuesActivityResponse
    {
        public string ActivityOutputValue { get; set; }
    }

    public class StaticBindFlowValuesActivity : IRequestHandler<StaticBindFlowValuesActivityRequest, StaticBindFlowValuesActivityResponse>
    {
        public Task<StaticBindFlowValuesActivityResponse> Handle(StaticBindFlowValuesActivityRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new StaticBindFlowValuesActivityResponse() { ActivityOutputValue = request.ActivityInputValue });
        }
    }
}
