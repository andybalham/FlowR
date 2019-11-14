using System;
using BusinessExample.Core;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;
using FlowR;
using MediatR;

namespace BusinessExample.Core.Exchanges.Checks
{
    public class CheckAffordability : FlowActivityRequest<CheckAffordability.Response>
    {
        public static FlowActivityDefinition<CheckAffordability, Response> NewDefinition() =>
            new FlowActivityDefinition<CheckAffordability, Response>();

        [BoundValue, NotNullValue]
        public LoanApplication LoanApplication { get; set; }

        [BoundValue, NotNullValue]
        public LoanDecision LoanDecision { get; set; }

        public class Response
        {
            public AffordabilityRating? AffordabilityRating { get; set; }
        }
    }

    public class CheckAffordabilityHandler : FlowRequestHandler<CheckAffordability, CheckAffordability.Response>
    {
        private readonly IAffordabilityEngine _affordabilityEngine;

        public CheckAffordabilityHandler(IAffordabilityEngine affordabilityEngine)
        {
            _affordabilityEngine = affordabilityEngine ?? throw new ArgumentNullException(nameof(affordabilityEngine));
        }

        public override CheckAffordability.Response Handle(CheckAffordability request)
        {
            var affordabilityRating = _affordabilityEngine.Calculate(request.LoanApplication);

            request.LoanDecision.AffordabilityRating = affordabilityRating;

            var response = new CheckAffordability.Response
            {
                AffordabilityRating = affordabilityRating
            };

            return response;
        }
    }

    public static class CheckAffordabilityMocks
    {
        public static FlowContext MockCheckAffordability(this FlowContext flowContext,
            AffordabilityRating affordabilityRating)
        {
            flowContext.MockActivity<CheckAffordability, CheckAffordability.Response>(
                req => new CheckAffordability.Response { AffordabilityRating = affordabilityRating });

            return flowContext;
        }
    }
}