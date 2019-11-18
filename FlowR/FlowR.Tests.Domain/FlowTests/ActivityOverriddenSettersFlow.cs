using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{

    public class ActivityOverriddenSettersFlowRequest : FlowActivityRequest<ActivityOverriddenSettersFlowResponse>
    {
        public static readonly string NonExistentSetterValue = Guid.NewGuid().ToString();
        public static readonly string DefaultBoundValue = Guid.NewGuid().ToString();
        public string FlowInputValue { get; set; }
        public string FlowNullValue { get; set; }
    }

    public class ActivityOverriddenSettersFlowResponse : FlowResponse
    {
        public string FlowOutputValue { get; set; }
        public string NonExistentSetterOutputValue { get; set; }
        public string NullSetterOutputValue { get; set; }
    }

    public class ActivityOverriddenSettersFlow : FlowHandler<ActivityOverriddenSettersFlowRequest, ActivityOverriddenSettersFlowResponse>
    {
        public ActivityOverriddenSettersFlow(IMediator mediator, IFlowLogger<ActivityOverriddenSettersFlow> logger) : base(mediator, logger)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()
                .Do("SetOutputValues", new FlowActivityDefinition<OverriddenSettersActivityRequest, OverriddenSettersActivityResponse>()
                    .BindInput(rq => rq.NonExistentSetterValue, "NonExistentValue")
                    .BindInput(rq => rq.BoundValueWithDefault, nameof(ActivityOverriddenSettersFlowRequest.FlowNullValue))
                    .BindInput(rq => rq.InputValue, nameof(ActivityOverriddenSettersFlowRequest.FlowInputValue)));
        }
    }


    public class OverriddenSettersActivityRequest : FlowActivityRequest<OverriddenSettersActivityResponse>
    {
        public string NonExistentSetterValue { get; set; } = ActivityOverriddenSettersFlowRequest.NonExistentSetterValue;

        public string BoundValueWithDefault { get; set; } = ActivityOverriddenSettersFlowRequest.DefaultBoundValue;

        public string InputValue { get; set; }
   }

    public class OverriddenSettersActivityResponse  
    {
        public string FlowOutputValue { get; set; }
        public string NonExistentSetterOutputValue { get; set; }
        public string NullFlowValueOutputValue { get; set; }
    }

    public class OverriddenSettersActivity : IRequestHandler<OverriddenSettersActivityRequest, OverriddenSettersActivityResponse>
    {
        public Task<OverriddenSettersActivityResponse> Handle(OverriddenSettersActivityRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new OverriddenSettersActivityResponse()
            {
                NonExistentSetterOutputValue = request.NonExistentSetterValue,
                NullFlowValueOutputValue = request.BoundValueWithDefault,
                FlowOutputValue = request.InputValue
            });
        }
    }
}
