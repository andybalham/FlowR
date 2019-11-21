using System;
using System.Collections.Generic;
using System.Text;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;

namespace BusinessExample.Infrastructure
{
    public class AffordabilityEngine : IAffordabilityEngine
    {
        public AffordabilityRating Calculate(LoanApplication loanApplication)
        {
            var monthlyDifference = loanApplication.MonthlyIncomeAmount - loanApplication.MonthlyOutgoingAmount;

            switch (monthlyDifference)
            {
                case decimal diff when diff >= 1000:
                    return AffordabilityRating.Good;

                case decimal diff when diff >= 0:
                    return AffordabilityRating.Fair;

                default:
                    return AffordabilityRating.Poor;
            }
        }
    }
}
