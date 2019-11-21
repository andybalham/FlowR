using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BusinessExample.Core;
using BusinessExample.Core.Entities;

namespace BusinessExample.Core.Interfaces
{
    public interface ILoanApplicationRepository
    {
        Task<string> CreateLoanApplication(LoanApplication loanApplication);

        Task<LoanApplication> RetrieveLoanApplication(string loanApplicationId);
    }
}
