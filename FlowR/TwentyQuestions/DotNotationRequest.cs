using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlowR;
using MediatR;

namespace TwentyQuestions
{
    public class DotNotationRequest : FlowActivityRequest<DotNotationResponse>
    {
        public static FlowActivityDefinition<DotNotationRequest, DotNotationResponse> NewDefinition() =>
            new FlowActivityDefinition<DotNotationRequest, DotNotationResponse>();

        public override string GetText() => $"Output DotNotation for\r\n{this.TargetRequestType.Name}";

        [NotNullValue]
        public Type TargetRequestType { get; set; }
    }

    public class DotNotationResponse
    {
    }

    public class DotNotationHandler : IRequestHandler<DotNotationRequest, DotNotationResponse>
    {
        private readonly IMediator _mediator;
        private readonly IConsoleService _console;

        public DotNotationHandler(IMediator mediator, IConsoleService console)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public Task<DotNotationResponse> Handle(DotNotationRequest request, CancellationToken cancellationToken)
        {
            var flowDiscoveryResponse =
                _mediator.Send(new FlowDiscoveryRequest(), cancellationToken)
                    .GetAwaiter().GetResult();

            var targetFlow = 
                flowDiscoveryResponse.Flows.First(f => f.Request.RequestType == request.TargetRequestType);

            _console.WriteLine("****************************************************");
            _console.WriteLine(targetFlow.GetDotNotation());
            _console.WriteLine("****************************************************");

            return Task.FromResult(new DotNotationResponse());
        }
    }
}
