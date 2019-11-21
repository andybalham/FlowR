using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessExample.Core.Interfaces
{
    public interface ILoanEventService
    {
        Task RaiseEvent(LoanEventType loanEventType, string loanApplicationId, string loanDecisionId);
    }

    public enum LoanEventType
    {
        Unknown = 0,
        Referred,
    }
}
