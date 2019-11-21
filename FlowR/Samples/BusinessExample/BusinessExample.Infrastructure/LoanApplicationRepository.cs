using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;

namespace BusinessExample.Infrastructure
{
    public class LoanApplicationRepository : ILoanApplicationRepository
    {
        private int _nextReference = 10000000;
        private readonly IDictionary<string, LoanApplication> _loanApplications = new Dictionary<string, LoanApplication>();

        public Task<string> CreateLoanApplication(LoanApplication loanApplication)
        {
            _nextReference += 1;

            loanApplication.Id = loanApplication.Id ?? Guid.NewGuid().ToString();
            loanApplication.Reference = _nextReference.ToString();

            _loanApplications.Add(loanApplication.Id, loanApplication);

            return Task.FromResult(loanApplication.Id);
        }

        public Task<LoanApplication> RetrieveLoanApplication(string loanApplicationId)
        {
            return _loanApplications.TryGetValue(loanApplicationId, out var loanApplication) 
                ? Task.FromResult(loanApplication) 
                : Task.FromResult<LoanApplication>(null);
        }
    }
}
