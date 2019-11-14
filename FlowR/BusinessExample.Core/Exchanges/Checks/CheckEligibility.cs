using System;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;
using FlowR;
using MediatR;

namespace BusinessExample.Core.Exchanges.Checks
{
    public class CheckEligibility : FlowActivityRequest<CheckEligibility.Response>
    {
        public static FlowActivityDefinition<CheckEligibility, Response> NewDefinition() =>
            new FlowActivityDefinition<CheckEligibility, Response>();

        [BoundValue, NotNullValue]
        public LoanApplication LoanApplication { get; set; }

        [BoundValue, NotNullValue]
        public LoanDecision LoanDecision { get; set; }

        public class Response
        {
            public bool? IsEligible { get; set; }
        }
    }

    public class CheckEligibilityHandler : FlowRequestHandler<CheckEligibility, CheckEligibility.Response>
    {
        private readonly IEligibilityVerifier _eligibilityVerifier;

        public CheckEligibilityHandler(IEligibilityVerifier eligibilityVerifier)
        {
            _eligibilityVerifier = eligibilityVerifier ?? throw new ArgumentNullException(nameof(eligibilityVerifier));
        }

        public override CheckEligibility.Response Handle(CheckEligibility request)
        {
            var isEligible = _eligibilityVerifier.IsEligible(request.LoanApplication);

            request.LoanDecision.IsEligible = isEligible;

            var response = new CheckEligibility.Response
            {
                IsEligible = isEligible
            };

            return response;
        }
    }

    public static class CheckEligibilityMocks
    {
        public static FlowContext MockCheckEligibility(this FlowContext flowContext, bool isEligible)
        {
            flowContext.MockActivity<CheckEligibility, CheckEligibility.Response>(
                req => new CheckEligibility.Response { IsEligible = isEligible });

            return flowContext;
        }
    }
}