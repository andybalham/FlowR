using System.Threading;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using FlowR;
using MediatR;

namespace BusinessExample.Core.Exchanges.LoanDecisions
{
    public class SetLoanDecisionResult : FlowActivityRequest<SetLoanDecisionResult.Response>
    {
        public static FlowActivityDefinition<SetLoanDecisionResult, Response> NewDefinition() =>
            new FlowActivityDefinition<SetLoanDecisionResult, Response>();

        public LoanDecisionResult Result { get; set; }

        [NotNullValue]
        public LoanDecision LoanDecision { get; set; }

        public class Response
        {
            public LoanDecisionResult Result { get; set; }
        }
    }

    public class SetLoanDecisionResultHandler : IRequestHandler<SetLoanDecisionResult, SetLoanDecisionResult.Response>
    {
        public Task<SetLoanDecisionResult.Response> Handle(SetLoanDecisionResult request, CancellationToken cancellationToken)
        {
            request.LoanDecision.Result = request.Result;

            var response = new SetLoanDecisionResult.Response { Result = request.Result };
            
            return Task.FromResult(response);
        }
    }
}