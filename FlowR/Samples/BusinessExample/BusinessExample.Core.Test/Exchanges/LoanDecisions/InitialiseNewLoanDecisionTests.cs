using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Exchanges.LoanDecisions;
using Xunit;

namespace BusinessExample.Core.Test.Exchanges.LoanDecisions
{
    public class InitialiseNewLoanDecisionTests
    {
        [Fact]
        public async void Can_initialise_new_loan_decision()
        {
            // Arrange

            var handler = new InitialiseNewLoanDecisionHandler();

            var request = new InitialiseNewLoanDecision
            {
                LoanApplication = new LoanApplication { Id = Guid.NewGuid().ToString() }
            };

            // Act

            var response = await handler.Handle(request, CancellationToken.None);

            // Assert

            Assert.NotNull(response.LoanDecision);
            Assert.NotNull(response.LoanDecision.Id);
            Assert.Equal(request.LoanApplication.Id, response.LoanDecision.LoanApplicationId);
        }
    }
}
