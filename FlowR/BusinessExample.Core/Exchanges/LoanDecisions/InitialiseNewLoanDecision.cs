using System;
using System.Threading;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using FlowR;
using MediatR;

namespace BusinessExample.Core.Exchanges.LoanDecisions
{
    public class InitialiseNewLoanDecision : FlowActivityRequest<InitialiseNewLoanDecision.Response>
    {
        public static FlowActivityDefinition<InitialiseNewLoanDecision, Response> NewDefinition() =>
            new FlowActivityDefinition<InitialiseNewLoanDecision, Response>();

        [BoundValue, NotNullValue]
        public LoanApplication LoanApplication { get; set; }

        public class Response
        {
            public LoanDecision LoanDecision { get; set; }
        }
    }

    public class InitialiseNewLoanDecisionHandler : IRequestHandler<InitialiseNewLoanDecision, InitialiseNewLoanDecision.Response>
    {
        public Task<InitialiseNewLoanDecision.Response> Handle(InitialiseNewLoanDecision request, CancellationToken cancellationToken)
        {
            var loanDecisionId = Guid.NewGuid().ToString();

            var response = new InitialiseNewLoanDecision.Response
            {
                LoanDecision = new LoanDecision
                {
                    Id = loanDecisionId,
                    LoanApplicationId = request.LoanApplication.Id
                }
            };

            return Task.FromResult(response);
        }
    }
}