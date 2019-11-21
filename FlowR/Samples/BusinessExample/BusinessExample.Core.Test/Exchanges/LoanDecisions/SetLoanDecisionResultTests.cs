using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Exchanges.LoanDecisions;
using Xunit;

namespace BusinessExample.Core.Test.Exchanges.LoanDecisions
{
    public class SetLoanDecisionResultTests
    {
        [Fact]
        public async void Can_set_loan_decision_result()
        {
            // Arrange

            var request = new SetLoanDecisionResult
            {
                Result = LoanDecisionResult.Accept,
                LoanDecision = new LoanDecision()
            };

            var handler = new SetLoanDecisionResultHandler();

            // Act

            var response = await handler.Handle(request, CancellationToken.None);

            // Assert

            Assert.Equal(request.Result, response.Result);
            Assert.Equal(request.Result, request.LoanDecision.Result);
        }
    }
}
