using System;
using BusinessExample.Core.Entities;
using Xunit;

namespace BusinessExample.Infrastructure.Test
{
    public class AffordabilityEngineTests
    {
        [Theory]
        [InlineData(1000, 0, AffordabilityRating.Good)]
        [InlineData(999.99, 0, AffordabilityRating.Fair)]
        [InlineData(0, 0, AffordabilityRating.Fair)]
        [InlineData(0, 0.01, AffordabilityRating.Poor)]
        public void Can_rate_loan_applications(decimal monthlyIncomeAmount, decimal monthlyOutgoingAmount, AffordabilityRating expectedRating)
        {
            var loanApplication = new LoanApplication
            {
                MonthlyIncomeAmount = monthlyIncomeAmount,
                MonthlyOutgoingAmount = monthlyOutgoingAmount,
            };

            var affordabilityEngine = new AffordabilityEngine();

            var actualRating = affordabilityEngine.Calculate(loanApplication);

            Assert.Equal(expectedRating, actualRating);
        }
    }
}
