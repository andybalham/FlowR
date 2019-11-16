using System;
using System.Threading;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;
using FlowR;
using MediatR;

namespace BusinessExample.Core.Exchanges.LoanDecisions
{
    public class CreateLoanDecision : FlowActivityRequest<CreateLoanDecision.Response>
    {
        public static FlowActivityDefinition<CreateLoanDecision, Response> NewDefinition() =>
            new FlowActivityDefinition<CreateLoanDecision, Response>();

        [BoundValue, NotNullValue]
        public LoanDecision LoanDecision { get; set; }

        public class Response
        {
            public string LoanDecisionId { get; set; }
        }
    }

    public class CreateLoanDecisionHandler : IRequestHandler<CreateLoanDecision, CreateLoanDecision.Response>
    {
        private readonly ILoanDecisionRepository _loanDecisionRepository;

        public CreateLoanDecisionHandler(ILoanDecisionRepository loanDecisionRepository)
        {
            _loanDecisionRepository = loanDecisionRepository ?? throw new ArgumentNullException(nameof(loanDecisionRepository));
        }

        public async Task<CreateLoanDecision.Response> Handle(CreateLoanDecision request, CancellationToken cancellationToken)
        {
            var loanDecisionId = await _loanDecisionRepository.CreateLoanDecision(request.LoanDecision);

            var response = new CreateLoanDecision.Response
            {
                LoanDecisionId = loanDecisionId
            };

            return response;
        }
    }

    public static class CreateLoanDecisionMocks
    {
        public static FlowContext MockCreateLoanDecision(this FlowContext flowContext,
            LoanDecisionResult?[] actualDecisionOverallResult = null)
        {
            flowContext.MockActivity<CreateLoanDecision, CreateLoanDecision.Response>(req =>
            {
                if (actualDecisionOverallResult != null)
                    actualDecisionOverallResult[0] = req.LoanDecision.Result;
                return new CreateLoanDecision.Response { LoanDecisionId = Guid.NewGuid().ToString() };
            });

            return flowContext;
        }
    }
}