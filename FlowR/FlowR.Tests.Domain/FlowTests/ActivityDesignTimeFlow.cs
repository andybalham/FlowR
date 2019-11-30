using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{

    public class ActivityDesignTimeFlowRequest : FlowActivityRequest<ActivityDesignTimeFlowResponse>
    {
    }

    public class ActivityDesignTimeFlowResponse : FlowResponse
    {
        public string OutputValue { get; set; }
    }

    public class ActivityDesignTimeFlow : FlowHandler<ActivityDesignTimeFlowRequest, ActivityDesignTimeFlowResponse>
    {
        public const string DesignTimeValue = "May the force be with you";

        public ActivityDesignTimeFlow(IMediator mediator, IFlowLogger<ActivityDesignTimeFlow> logger) : base(mediator, logger)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition<ActivityDesignTimeFlowRequest, ActivityDesignTimeFlowResponse> flowDefinition)
        {
            flowDefinition
                .Do("Set_output_from_design_time", new FlowActivityDefinition<DesignTimeActivityRequest, DesignTimeActivityResponse>()
                    .SetValue(rq => rq.DesignTimeValue, DesignTimeValue));
        }
    }


    public class DesignTimeActivityRequest : FlowActivityRequest<DesignTimeActivityResponse>
    {
        public string DesignTimeValue { get; set; }
    }

    public class DesignTimeActivityResponse
    {
        public string OutputValue { get; set; }
    }

    public class DesignTimeActivity : IRequestHandler<DesignTimeActivityRequest, DesignTimeActivityResponse>
    {
        public Task<DesignTimeActivityResponse> Handle(DesignTimeActivityRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new DesignTimeActivityResponse() {OutputValue = request.DesignTimeValue});
        }
    }
}