using System;
using System.Collections.Generic;
using System.Text;
using BusinessExample.Core.Entities;
using Xunit;

namespace BusinessExample.Infrastructure.Test
{
    public class IdentityServiceTests
    {
        [Theory]
        [InlineData("Ray Purchase", IdentityCheckResult.IdentityFound)]
        [InlineData("Clem Unknown", IdentityCheckResult.IdentityNotFound)]
        [InlineData("Frank Unavailable", IdentityCheckResult.ServiceUnavailable)]
        public async void Can_check_identity(string applicantName, IdentityCheckResult expectedResult)
        {
            // Arrange

            var loanApplication = new LoanApplication
            {
                ApplicantName = applicantName
            };

            var sut = new IdentityService();

            // Act

            var actualResult = await sut.CheckIdentity(loanApplication);

            // Assert

            Assert.Equal(expectedResult, actualResult);
        }
    }
}
