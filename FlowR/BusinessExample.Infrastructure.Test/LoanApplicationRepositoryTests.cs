using System;
using System.Collections.Generic;
using System.Text;
using BusinessExample.Core.Entities;
using Xunit;

namespace BusinessExample.Infrastructure.Test
{
    public class LoanApplicationRepositoryTests
    {
        [Fact]
        public async void Can_create_and_retrieve_loan_application()
        {
            // Arrange

            var loanApplication = new LoanApplication
            {
                EmailAddress = "barnaby.fudge@andybalham.com"
            };

            var sut = new LoanApplicationRepository();

            // Act

            var loanApplicationId = await sut.CreateLoanApplication(loanApplication);

            var retrievedLoanApplication = await sut.RetrieveLoanApplication(loanApplicationId);

            // Assert

            Assert.NotEmpty(retrievedLoanApplication.Reference);
            Assert.Equal(loanApplication.EmailAddress, retrievedLoanApplication.EmailAddress);
        }

        [Fact]
        public async void Can_create_and_retrieve_loan_application_with_id()
        {
            // Arrange

            var loanApplication = new LoanApplication
            {
                Id = Guid.NewGuid().ToString(),
                EmailAddress = "barnaby.fudge@andybalham.com"
            };

            var sut = new LoanApplicationRepository();

            // Act

            var loanApplicationId = await sut.CreateLoanApplication(loanApplication);

            Assert.Equal(loanApplication.Id, loanApplicationId);

            var retrievedLoanApplication = await sut.RetrieveLoanApplication(loanApplicationId);

            // Assert

            Assert.Equal(loanApplication.EmailAddress, retrievedLoanApplication.EmailAddress);
        }

        [Fact]
        public async void Null_returned_for_non_existent_loan_application()
        {
            // Arrange

            var sut = new LoanApplicationRepository();

            // Act

            var retrievedLoanApplication = await sut.RetrieveLoanApplication(Guid.NewGuid().ToString());

            // Assert

            Assert.Null(retrievedLoanApplication);
        }
    }
}
