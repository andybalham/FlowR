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

        public override string GetText() => $"Output DotNotation for {this.TargetRequestType.Name}";

        [NotNullValue]
        public Type TargetRequestType { get; set; }
    }

    public class DotNotationResponse
    {
    }

    public class DotNotationHandler : IRequestHandler<DotNotationRequest, DotNotationResponse>
    {
        private readonly IMediator _mediator;

        public DotNotationHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<DotNotationResponse> Handle(DotNotationRequest request, CancellationToken cancellationToken)
        {
            var flowDiscoveryResponse =
                _mediator.Send(new FlowDiscoveryRequest(), cancellationToken)
                    .GetAwaiter().GetResult();

            var twentyQuestionsFlow =
                flowDiscoveryResponse.Flows.First(f => f.Request.RequestType == request.TargetRequestType);

            Console.WriteLine("****************************************************");
            Console.WriteLine(twentyQuestionsFlow.GetDotNotation());
            Console.WriteLine("****************************************************");

            return Task.FromResult(new DotNotationResponse());
        }
    }
}
