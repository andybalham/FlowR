using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace FlowR.Tests.Domain.FlowrBasedMockingTests
{
    public class CanMockOnlyRootFlowRequest : FlowActivityRequest<CanMockOnlyRootFlowResponse>
    {
    }

    public class CanMockOnlyRootFlowResponse
    {
        public bool Value { get; set; }
    }

    public class CanMockOnlyRootFlowHandler : FlowHandler<CanMockOnlyRootFlowRequest, CanMockOnlyRootFlowResponse>
    {
        public CanMockOnlyRootFlowHandler(IMediator mediator) : base(mediator)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
            flowDefinition
                .Do("CallSubFlow", 
                    new FlowActivityDefinition<CanMockOnlyRootSubFlowRequest, CanMockOnlyRootSubFlowResponse>());
        }
    }

    public class CanMockOnlyRootSubFlowRequest : FlowActivityRequest<CanMockOnlyRootSubFlowResponse>
    {
    }

    public class CanMockOnlyRootSubFlowResponse
    {
        public bool Value { get; set; }
    }

    public class CanMockOnlyRootSubFlowHandler : FlowHandler<CanMockOnlyRootSubFlowRequest, CanMockOnlyRootSubFlowResponse>
    {
        public CanMockOnlyRootSubFlowHandler(IMediator mediator) : base(mediator)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
            flowDefinition
                .Do("SetValue", 
                    new FlowActivityDefinition<CanMockOnlyRootSetValueRequest, CanMockOnlyRootSetValueResponse>());
        }
    }

    public class CanMockOnlyRootSetValueRequest : FlowActivityRequest<CanMockOnlyRootSetValueResponse>
    {
    }

    public class CanMockOnlyRootSetValueResponse
    {
        public bool Value { get; set; }
    }

    public class CanMockOnlyRootSetValueHandler : IRequestHandler<CanMockOnlyRootSetValueRequest, CanMockOnlyRootSetValueResponse>
    {
        public Task<CanMockOnlyRootSetValueResponse> Handle(CanMockOnlyRootSetValueRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new CanMockOnlyRootSetValueResponse { Value = true });
        }
    }
}
