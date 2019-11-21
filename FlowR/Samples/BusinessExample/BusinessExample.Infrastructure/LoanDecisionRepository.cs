using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;

namespace BusinessExample.Infrastructure
{
    public class LoanDecisionRepository : ILoanDecisionRepository
    {
        private readonly IDictionary<string, LoanDecision> _loanDecisions = new Dictionary<string, LoanDecision>();

        public Task<string> CreateLoanDecision(LoanDecision loanDecision)
        {
            loanDecision.Id = loanDecision.Id ?? Guid.NewGuid().ToString();

            _loanDecisions.Add(loanDecision.Id, loanDecision);

            return Task.FromResult(loanDecision.Id);
        }

        public Task<LoanDecision> RetrieveLoanDecision(string loanDecisionId)
        {
            return _loanDecisions.TryGetValue(loanDecisionId, out var loanDecision) 
                ? Task.FromResult(loanDecision) 
                : Task.FromResult<LoanDecision>(null);
        }
    }
}
