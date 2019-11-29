using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{

    public class ActivityLoggingFlowRequest : FlowActivityRequest<ActivityLoggingFlowResponse>
    {
        public Guid PublicInput { get; set; }
        [SensitiveValue]
        public Guid PrivateInput { get; set; }
    }

    public class ActivityLoggingFlowResponse : FlowResponse
    {
        public Guid PublicOutput { get; set; }
        [SensitiveValue]
        public Guid PrivateOutput { get; set; }
    }

    public class ActivityLoggingFlow : FlowHandler<ActivityLoggingFlowRequest, ActivityLoggingFlowResponse>
    {
        public ActivityLoggingFlow(IMediator mediator, IFlowLogger<ActivityLoggingFlow> logger) : base(mediator, logger)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
            flowDefinition
                .Do("LoggedActivity", new FlowActivityDefinition<LoggedActivityRequest, LoggedActivityResponse>());
        }
    }

    public class LoggedActivityRequest : FlowActivityRequest<LoggedActivityResponse>
    {
        public Guid PublicInput { get; set; }
        [SensitiveValue]
        public Guid PrivateInput { get; set; }
    }

    public class LoggedActivityResponse
    {
        public Guid PublicOutput { get; set; }
        [SensitiveValue]
        public Guid PrivateOutput { get; set; }
    }

    public class LoggedActivity : IRequestHandler<LoggedActivityRequest, LoggedActivityResponse>
    {
        public Task<LoggedActivityResponse> Handle(LoggedActivityRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new LoggedActivityResponse { PublicOutput = request.PublicInput, PrivateOutput = request.PrivateInput});
        }
    }
}
