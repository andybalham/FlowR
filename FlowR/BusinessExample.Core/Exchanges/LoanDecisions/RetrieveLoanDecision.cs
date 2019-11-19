using System;
using System.Threading;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Exceptions;
using BusinessExample.Core.Exchanges.LoanApplications;
using BusinessExample.Core.Interfaces;
using FlowR;
using MediatR;

namespace BusinessExample.Core.Exchanges.LoanDecisions
{
    public class RetrieveLoanDecision : FlowActivityRequest<RetrieveLoanDecision.Response>
    {
        [NotNullValue]
        public string LoanDecisionId { get; set; }

        public class Response : FlowResponse
        {
            public LoanDecision LoanDecision { get; set; }
        }
    }

    public class RetrieveLoanDecisionHandler : IRequestHandler<RetrieveLoanDecision, RetrieveLoanDecision.Response>
    {
        private readonly ILoanDecisionRepository _repository;

        public RetrieveLoanDecisionHandler(ILoanDecisionRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<RetrieveLoanDecision.Response> Handle(RetrieveLoanDecision request, CancellationToken cancellationToken)
        {
            var loanDecision = await _repository.RetrieveLoanDecision(request.LoanDecisionId);

            if (loanDecision == null)
            {
                throw new NotFoundResourceException();
            }

            var response = new RetrieveLoanDecision.Response
            {
                LoanDecision = loanDecision
            };

            return response;
        }
    }
}
