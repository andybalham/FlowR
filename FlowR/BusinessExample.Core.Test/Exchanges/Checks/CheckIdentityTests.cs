using System.Threading;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Exchanges.Checks;
using BusinessExample.Core.Interfaces;
using Moq;
using Xunit;

namespace BusinessExample.Core.Test.Exchanges.Checks
{
    public class CheckIdentityTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Eligibility_check_returns_value(bool verifierResult)
        {
            // Arrange

            var loanDecision = new LoanDecision();

            var request = new CheckEligibility
            {
                LoanApplication = new LoanApplication(),
                LoanDecision = loanDecision,
            };

            var mockEligibilityVerifier = new Mock<IEligibilityVerifier>(MockBehavior.Strict);
            mockEligibilityVerifier.Setup(m => m.IsEligible(It.IsNotNull<LoanApplication>())).Returns(verifierResult);

            var eligibilityCheck = new CheckEligibilityHandler(mockEligibilityVerifier.Object);

            // Act

            var response = eligibilityCheck.Handle(request);

            // Assert

            Assert.Equal(verifierResult, response.IsEligible);
            Assert.Equal(verifierResult, loanDecision.IsEligible);
        }
    }
}
