using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Exceptions;
using BusinessExample.Core.Exchanges.LoanApplications;
using BusinessExample.Core.Interfaces;
using Moq;
using Xunit;

namespace BusinessExample.Core.Test.Exchanges.LoanApplications
{
    public class LoanDecisionCrudTests
    {
        [Fact]
        public async void Can_create_loan_application()
        {
            // Arrange

            var repositoryMock = new Mock<ILoanApplicationRepository>(MockBehavior.Strict);

            repositoryMock.Setup(m => m.CreateLoanApplication(It.IsNotNull<LoanApplication>()))
                .ReturnsAsync((LoanApplication la) => la.Id);

            var handler = new CreateLoanApplicationHandler(repositoryMock.Object);

            var request = new CreateLoanApplication
            {
                LoanApplication = new LoanApplication { Id = Guid.NewGuid().ToString() }
            };

            // Act

            var response = await handler.Handle(request, CancellationToken.None);

            // Assert

            Assert.Equal(request.LoanApplication.Id, response.LoanApplicationId);
        }

        [Fact]
        public async void Can_retrieve_existing_loan_application()
        {
            // Arrange

            var repositoryMock = new Mock<ILoanApplicationRepository>(MockBehavior.Strict);

            repositoryMock.Setup(m => m.RetrieveLoanApplication(It.IsNotNull<string>()))
                .ReturnsAsync((string id) => new LoanApplication { Id = id });

            var handler = new RetrieveLoanApplicationHandler(repositoryMock.Object);

            var request = new RetrieveLoanApplication
            {
                LoanApplicationId = Guid.NewGuid().ToString()
            };

            // Act

            var response = await handler.Handle(request, CancellationToken.None);

            // Assert

            Assert.Equal(request.LoanApplicationId, response.LoanApplication.Id);
        }

        [Fact]
        public async void Resource_exception_when_non_existent_loan_application()
        {
            // Arrange

            var repositoryMock = new Mock<ILoanApplicationRepository>(MockBehavior.Strict);

            repositoryMock.Setup(m => m.RetrieveLoanApplication(It.IsNotNull<string>()))
                .ReturnsAsync((string id) => null);

            var handler = new RetrieveLoanApplicationHandler(repositoryMock.Object);

            var request = new RetrieveLoanApplication
            {
                LoanApplicationId = Guid.NewGuid().ToString()
            };

            // Act

            await Assert.ThrowsAsync<NotFoundResourceException>(() => handler.Handle(request, CancellationToken.None));
        }
    }
}
