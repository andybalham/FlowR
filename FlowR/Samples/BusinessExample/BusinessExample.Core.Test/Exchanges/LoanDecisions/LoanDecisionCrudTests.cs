using System;
using System.Threading;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Exceptions;
using BusinessExample.Core.Exchanges.LoanDecisions;
using BusinessExample.Core.Interfaces;
using Moq;
using Xunit;

namespace BusinessExample.Core.Test.Exchanges.LoanDecisions
{
    public class LoanDecisionCrudTests
    {
        [Fact]
        public async void Can_create_loan_decision()
        {
            // Arrange

            var repositoryMock = new Mock<ILoanDecisionRepository>(MockBehavior.Strict);

            repositoryMock.Setup(m => m.CreateLoanDecision(It.IsNotNull<LoanDecision>()))
                .ReturnsAsync((LoanDecision la) => la.Id);

            var handler = new CreateLoanDecisionHandler(repositoryMock.Object);

            var request = new CreateLoanDecision
            {
                LoanDecision = new LoanDecision { Id = Guid.NewGuid().ToString() }
            };

            // Act

            var response = await handler.Handle(request, CancellationToken.None);

            // Assert

            Assert.Equal(request.LoanDecision.Id, response.LoanDecisionId);
        }

        [Fact]
        public async void Can_retrieve_loan_decision()
        {
            // Arrange

            var repositoryMock = new Mock<ILoanDecisionRepository>(MockBehavior.Strict);

            repositoryMock.Setup(m => m.RetrieveLoanDecision(It.IsNotNull<string>()))
                .ReturnsAsync((string id) => new LoanDecision { Id = id });

            var handler = new RetrieveLoanDecisionHandler(repositoryMock.Object);

            var request = new RetrieveLoanDecision
            {
                LoanDecisionId = Guid.NewGuid().ToString()
            };

            // Act

            var response = await handler.Handle(request, CancellationToken.None);

            // Assert

            Assert.Equal(request.LoanDecisionId, response.LoanDecision.Id);
        }

        [Fact]
        public async void Resource_exception_when_non_existent_loan_decision()
        {
            // Arrange

            var repositoryMock = new Mock<ILoanDecisionRepository>(MockBehavior.Strict);

            repositoryMock.Setup(m => m.RetrieveLoanDecision(It.IsNotNull<string>()))
                .ReturnsAsync((string id) => null);

            var handler = new RetrieveLoanDecisionHandler(repositoryMock.Object);

            var request = new RetrieveLoanDecision
            {
                LoanDecisionId = Guid.NewGuid().ToString()
            };

            // Act

            await Assert.ThrowsAsync<NotFoundResourceException>(() => handler.Handle(request, CancellationToken.None));
        }
    }
}
