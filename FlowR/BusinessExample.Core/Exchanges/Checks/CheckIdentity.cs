using System;
using System.Threading;
using System.Threading.Tasks;
using BusinessExample.Core;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;
using FlowR;
using MediatR;

namespace BusinessExample.Core.Exchanges.Checks
{
    public class CheckIdentity : FlowActivityRequest<CheckIdentity.Response>
    {
        [BoundValue, NotNullValue]
        public LoanApplication LoanApplication { get; set; }


        [BoundValue, NotNullValue]
        public LoanDecision LoanDecision { get; set; }

        public class Response
        {
            public IdentityCheckResult? IdentityCheckResult { get; set; }
        }
    }

    public class CheckIdentityHandler : IRequestHandler<CheckIdentity, CheckIdentity.Response>
    {
        private readonly IIdentityService _identityService;

        public CheckIdentityHandler(IIdentityService identityService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        public async Task<CheckIdentity.Response> Handle(CheckIdentity request, CancellationToken cancellationToken)
        {
            var identityCheckResult = await _identityService.CheckIdentity(request.LoanApplication);

            request.LoanDecision.IdentityCheckResult = identityCheckResult;

            var response = new CheckIdentity.Response
            {
                IdentityCheckResult = identityCheckResult
            };

            return response;
        }
    }

    public class IdentityCheckResultDecision : NullableFlowValueDecision<IdentityCheckResult?>
    {
    }

    public class IdentityCheckResultDecisionHandler : FlowValueDecisionHandler<IdentityCheckResultDecision, IdentityCheckResult?>
    {
    }

    public static class CheckIdentityMocks
    {
        public static FlowContext MockIdentityCheck(this FlowContext flowContext, IdentityCheckResult identityCheckResult)
        {
            flowContext.MockActivity<CheckIdentity, CheckIdentity.Response>(
                req => new CheckIdentity.Response { IdentityCheckResult = identityCheckResult });

            return flowContext;
        }
    }
}