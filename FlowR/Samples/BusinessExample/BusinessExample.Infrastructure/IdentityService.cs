using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;

namespace BusinessExample.Infrastructure
{
    public class IdentityService : IIdentityService
    {
        public Task<IdentityCheckResult> CheckIdentity(LoanApplication loanApplication)
        {
            switch (loanApplication.ApplicantName)
            {
                case var name when Regex.IsMatch(name, "unknown", RegexOptions.IgnoreCase):
                    return Task.FromResult(IdentityCheckResult.IdentityNotFound);

                case var name when Regex.IsMatch(name, "unavailable", RegexOptions.IgnoreCase):
                    return Task.FromResult(IdentityCheckResult.ServiceUnavailable);

                default:
                    return Task.FromResult(IdentityCheckResult.IdentityFound);
            }
        }
    }
}
