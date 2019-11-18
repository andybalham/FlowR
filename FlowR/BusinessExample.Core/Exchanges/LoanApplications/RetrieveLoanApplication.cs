using System;
using System.Threading;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Exceptions;
using BusinessExample.Core.Interfaces;
using FlowR;
using MediatR;

namespace BusinessExample.Core.Exchanges.LoanApplications
{
    public class RetrieveLoanApplication : FlowActivityRequest<RetrieveLoanApplication.Response>
    {
        [NotNullValue]
        public string LoanApplicationId { get; set; }

        public class Response
        {
            public LoanApplication LoanApplication { get; set; }
        }
    }

    public class RetrieveLoanApplicationHandler : IRequestHandler<RetrieveLoanApplication, RetrieveLoanApplication.Response>
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;

        public RetrieveLoanApplicationHandler(ILoanApplicationRepository loanApplicationRepository)
        {
            _loanApplicationRepository = 
                loanApplicationRepository ?? throw new ArgumentNullException(nameof(loanApplicationRepository));
        }

        public async Task<RetrieveLoanApplication.Response> Handle(RetrieveLoanApplication request, CancellationToken cancellationToken)
        {
            var loanApplication = 
                await _loanApplicationRepository.RetrieveLoanApplication(request.LoanApplicationId);

            if (loanApplication == null)
            {
                throw new NotFoundResourceException();
            }

            var response = new RetrieveLoanApplication.Response
            {
                LoanApplication = loanApplication
            };

            return response;
        }
    }
}