using System;
using System.Collections.Generic;
using System.Text;
using BusinessExample.Core.Entities;
using Xunit;

namespace BusinessExample.Infrastructure.Test
{
    public class LoanDecisionRepositoryTests
    {
        [Fact]
        public async void Can_create_and_retrieve_loan_application()
        {
            // Arrange

            var loanDecision = new LoanDecision
            {
                Result = LoanDecisionResult.Accept
            };

            var sut = new LoanDecisionRepository();

            // Act

            var loanDecisionId = await sut.CreateLoanDecision(loanDecision);

            var retrievedLoanDecision = await sut.RetrieveLoanDecision(loanDecisionId);

            // Assert

            Assert.Equal(loanDecision.Result, retrievedLoanDecision.Result);
        }

        [Fact]
        public async void Can_create_and_retrieve_loan_application_with_id()
        {
            // Arrange

            var loanDecision = new LoanDecision
            {
                Id = Guid.NewGuid().ToString(),
                Result = LoanDecisionResult.Accept
            };

            var sut = new LoanDecisionRepository();

            // Act

            var loanDecisionId = await sut.CreateLoanDecision(loanDecision);

            Assert.Equal(loanDecision.Id, loanDecisionId);

            var retrievedLoanDecision = await sut.RetrieveLoanDecision(loanDecisionId);

            // Assert

            Assert.Equal(loanDecision.Result, retrievedLoanDecision.Result);
        }

        [Fact]
        public async void Null_returned_for_non_existent_loan_application()
        {
            // Arrange

            var sut = new LoanDecisionRepository();

            // Act

            var retrievedLoanDecision = await sut.RetrieveLoanDecision(Guid.NewGuid().ToString());

            // Assert

            Assert.Null(retrievedLoanDecision);
        }
    }
}
