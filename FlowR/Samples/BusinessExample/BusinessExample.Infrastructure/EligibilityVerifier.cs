using System;
using System.Collections.Generic;
using System.Text;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;

namespace BusinessExample.Infrastructure
{
    public class EligibilityVerifier : IEligibilityVerifier
    {
        public bool IsEligible(LoanApplication loanApplication)
        {
            return loanApplication.IsEmployed;
        }
    }
}
