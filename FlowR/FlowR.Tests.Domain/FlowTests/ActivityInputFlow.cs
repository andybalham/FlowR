using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlowR.Tests.Domain.FlowTests
{

    public class ActivityInputFlowRequest : FlowActivityRequest<ActivityInputFlowResponse>
    {
        public Guid InputValue { get; set; }
    }

    public class ActivityInputFlowResponse : FlowResponse
    {
        public Guid OutputValue { get; set; }
    }

    public class ActivityInputFlow : FlowHandler<ActivityInputFlowRequest, ActivityInputFlowResponse>
    {
        public ActivityInputFlow(IMediator mediator, IFlowLogger<ActivityInputFlow> logger) : base(mediator, logger)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
            flowDefinition
                .Do("InputAndOutput", new FlowActivityDefinition<InputAndOutputActivityRequest, InputAndOutputActivityResponse>());
        }
    }


    public class InputAndOutputActivityRequest : FlowActivityRequest<InputAndOutputActivityResponse>
    {
        public Guid InputValue { get; set; }
    }

    public class InputAndOutputActivityResponse
    {
        public Guid OutputValue { get; set; }
    }

    public class InputAndOutputActivity : IRequestHandler<InputAndOutputActivityRequest, InputAndOutputActivityResponse>
    {
        public Task<InputAndOutputActivityResponse> Handle(InputAndOutputActivityRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new InputAndOutputActivityResponse { OutputValue = request.InputValue });
        }
    }
}
