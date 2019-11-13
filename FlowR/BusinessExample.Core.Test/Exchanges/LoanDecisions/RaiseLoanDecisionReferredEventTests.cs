using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Exchanges.LoanDecisions;
using BusinessExample.Core.Interfaces;
using Moq;
using Xunit;

namespace BusinessExample.Core.Test.Exchanges.LoanDecisions
{
    public class RaiseLoanDecisionReferredEventTests
    {
        [Fact]
        public async void Can_raise_loan_referred_event()
        {
            // Arrange

            var request = new RaiseLoanDecisionReferredEvent
            {
                LoanDecision = new LoanDecision { Id = Guid.NewGuid().ToString() },
                LoanApplication = new LoanApplication { Id = Guid.NewGuid().ToString() }
            };

            var loanEventServiceMock = new Mock<ILoanEventService>(MockBehavior.Strict);

            loanEventServiceMock.Setup(m => 
                m.RaiseEvent(
                    It.Is<LoanEventType>(v => v == LoanEventType.Referred), 
                    It.Is<string>(v => v == request.LoanApplication.Id), 
                    It.Is<string>(v => v == request.LoanDecision.Id)))
                .Returns(Task.CompletedTask);

            var handler = new RaiseLoanDecisionReferredEventHandler(loanEventServiceMock.Object);

            // Act

            await handler.Handle(request, CancellationToken.None);
        }
    }
}
