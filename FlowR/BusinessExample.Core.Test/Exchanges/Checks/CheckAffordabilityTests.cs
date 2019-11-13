using BusinessExample.Core.Entities;
using BusinessExample.Core.Exchanges.Checks;
using BusinessExample.Core.Interfaces;
using Moq;
using Xunit;

namespace BusinessExample.Core.Test.Exchanges.Checks
{
    public class CheckAffordabilityTests
    {
        [Theory]
        [InlineData(AffordabilityRating.Poor)]
        [InlineData(AffordabilityRating.Good)]
        public void Affordability_check_returns_value(AffordabilityRating engineResult)
        {
            // Arrange

            var loanDecision = new LoanDecision();

            var request = new CheckAffordability
            {
                LoanApplication = new LoanApplication(),
                LoanDecision = loanDecision,
            };

            var mockAffordabilityVerifier = new Mock<IAffordabilityEngine>(MockBehavior.Strict);
            mockAffordabilityVerifier.Setup(m => m.Calculate(It.IsNotNull<LoanApplication>())).Returns(engineResult);

            var affordabilityCheck = new CheckAffordabilityHandler(mockAffordabilityVerifier.Object);

            // Act

            var response = affordabilityCheck.Handle(request);

            // Assert

            Assert.Equal(engineResult, response.AffordabilityRating);
            Assert.Equal(engineResult, loanDecision.AffordabilityRating);
        }
    }
}
