using System;
using System.Threading;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;
using FlowR;
using MediatR;

namespace BusinessExample.Core.Exchanges.LoanApplications
{
    public class CreateLoanApplication : FlowActivityRequest<CreateLoanApplication.Response>
    {
        [NotNullValue]
        public LoanApplication LoanApplication { get; set; }

        public class Response
        {
            public string LoanApplicationId { get; set; }
        }
    }

    public class CreateLoanApplicationHandler : IRequestHandler<CreateLoanApplication, CreateLoanApplication.Response>
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;

        public CreateLoanApplicationHandler(ILoanApplicationRepository loanApplicationRepository)
        {
            _loanApplicationRepository = loanApplicationRepository ?? throw new ArgumentNullException(nameof(loanApplicationRepository));
        }

        public async Task<CreateLoanApplication.Response> Handle(CreateLoanApplication request, CancellationToken cancellationToken)
        {
            var loanApplicationId = await _loanApplicationRepository.CreateLoanApplication(request.LoanApplication);

            return new CreateLoanApplication.Response
            {
                LoanApplicationId = loanApplicationId
            };
        }
    }
}
