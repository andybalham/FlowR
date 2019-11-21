using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BusinessExample.Core;
using BusinessExample.Core.Entities;

namespace BusinessExample.Core.Interfaces
{
    public interface ILoanDecisionRepository
    {
        Task<string> CreateLoanDecision(LoanDecision loanDecision);
        
        Task<LoanDecision> RetrieveLoanDecision(string loanDecisionId);
    }
}
