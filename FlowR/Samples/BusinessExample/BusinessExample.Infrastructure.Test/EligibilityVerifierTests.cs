using System;
using System.Collections.Generic;
using System.Text;
using BusinessExample.Core.Entities;
using Xunit;

namespace BusinessExample.Infrastructure.Test
{
    public class EligibilityVerifierTests
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public void Can_verify_eligibility(bool isEmployed, bool expectedEligibility)
        {
            // Arrange

            var loanApplication = new LoanApplication
            {
                IsEmployed = isEmployed
            };

            var sut = new EligibilityVerifier();

            // Act

            var isEligible = sut.IsEligible(loanApplication);

            // Assert

            Assert.Equal(expectedEligibility, isEligible);
        }
    }
}
