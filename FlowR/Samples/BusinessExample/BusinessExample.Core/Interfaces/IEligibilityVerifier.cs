using System;
using System.Collections.Generic;
using System.Text;
using BusinessExample.Core;
using BusinessExample.Core.Entities;

namespace BusinessExample.Core.Interfaces
{
    public interface IEligibilityVerifier
    {
        bool IsEligible(LoanApplication loanApplication);
    }
}
