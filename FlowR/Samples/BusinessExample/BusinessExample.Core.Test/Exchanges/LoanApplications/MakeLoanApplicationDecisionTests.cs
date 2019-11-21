using System;
using System.Collections.Generic;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Exchanges.Checks;
using BusinessExample.Core.Exchanges.Communications;
using BusinessExample.Core.Exchanges.LoanApplications;
using BusinessExample.Core.Exchanges.LoanDecisions;
using FlowR;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace BusinessExample.Core.Test.Exchanges.LoanApplications
{
    public class MakeLoanApplicationDecisionTests : FlowHandlerTestBase
    {
        public MakeLoanApplicationDecisionTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(false, AffordabilityRating.Good, IdentityCheckResult.IdentityFound)]
        [InlineData(true, AffordabilityRating.Poor, IdentityCheckResult.IdentityFound)]
        [InlineData(true, AffordabilityRating.Good, IdentityCheckResult.IdentityNotFound)]
        public async void Application_is_declined(
            bool isEligible, AffordabilityRating affordabilityRating, IdentityCheckResult identityCheckResult)
        {
            // Arrange

            var loanApplication = BuildLoanApplication();

            var actualTemplateNames = new List<string>();

            var flowContext = new FlowContext()
                .MockCheckEligibility(isEligible)
                .MockCheckAffordability(affordabilityRating)
                .MockIdentityCheck(identityCheckResult)
                .MockSendEmail(actualTemplateNames)
                .MockCreateLoanDecision();

            var request = new MakeLoanApplicationDecision
            {
                FlowContext = flowContext,
                LoanApplication = loanApplication
            };

            // Act

            var (mediator, logger) = GetMediator(request);

            var response = await mediator.Send(request);

            logger.LogDebug($"Trace: {response.Trace}");

            // Assert

            Assert.NotNull(response.LoanDecision);
            Assert.Equal(LoanDecisionResult.Decline, response.LoanDecision.Result);

            Assert.Single(actualTemplateNames);
            Assert.Contains("DeclineConfirmation", actualTemplateNames);
        }

        [Theory]
        [InlineData(true, AffordabilityRating.Fair, IdentityCheckResult.IdentityFound)]
        [InlineData(true, AffordabilityRating.Good, IdentityCheckResult.ServiceUnavailable)]
        public async void Application_is_referred(
            bool isEligible, AffordabilityRating affordabilityRating, IdentityCheckResult identityCheckResult)
        {
            // Arrange

            var loanApplication = BuildLoanApplication();

            var actualTemplateNames = new List<string>();
            var isLoanReferredEventRaised = new bool?[1];

            var flowContext = new FlowContext()
                .MockCheckEligibility(isEligible)
                .MockCheckAffordability(affordabilityRating)
                .MockIdentityCheck(identityCheckResult)
                .MockSendEmail(actualTemplateNames)
                .MockCreateLoanDecision()
                .MockRaiseLoanDecisionReferredEvent(isLoanReferredEventRaised);

            var request = new MakeLoanApplicationDecision
            {
                FlowContext = flowContext,
                LoanApplication = loanApplication
            };

            // Act

            var (mediator, logger) = GetMediator(request);

            var response = await mediator.Send(request);

            logger.LogDebug($"Trace: {response.Trace}");

            // Assert

            Assert.NotNull(response.LoanDecision);
            Assert.Equal(LoanDecisionResult.Refer, response.LoanDecision.Result);

            Assert.True(isLoanReferredEventRaised[0]);
            Assert.Single(actualTemplateNames);
            Assert.Contains("ReferNotification", actualTemplateNames);
        }

        [Theory]
        [InlineData(true, AffordabilityRating.Good, IdentityCheckResult.IdentityFound)]
        public async void Application_is_accepted(
            bool isEligible, AffordabilityRating affordabilityRating, IdentityCheckResult identityCheckResult)
        {
            // Arrange

            var loanApplication = BuildLoanApplication();

            var actualTemplateNames = new List<string>();

            var flowContext = new FlowContext()
                .MockCheckEligibility(isEligible)
                .MockCheckAffordability(affordabilityRating)
                .MockIdentityCheck(identityCheckResult)
                .MockSendEmail(actualTemplateNames)
                .MockCreateLoanDecision();

            var request = new MakeLoanApplicationDecision
            {
                FlowContext = flowContext,
                LoanApplication = loanApplication
            };

            // Act

            var (mediator, logger) = GetMediator(request);

            var response = await mediator.Send(request);

            logger.LogDebug($"Trace: {response.Trace}");

            // Assert

            Assert.NotNull(response.LoanDecision);
            Assert.Equal(LoanDecisionResult.Accept, response.LoanDecision.Result);
            
            Assert.Single(actualTemplateNames);
            Assert.Contains("AcceptConfirmation", actualTemplateNames);
        }

        private static LoanApplication BuildLoanApplication()
        {
            return new LoanApplication
            {
                Id = Guid.NewGuid().ToString(),
                Reference = "M1000002772",
                EmailAddress = "bananas@andybalham.com",
            };
        }
    }
}
