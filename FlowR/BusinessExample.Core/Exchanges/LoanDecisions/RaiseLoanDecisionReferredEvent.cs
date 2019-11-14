using System;
using System.Threading;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;
using FlowR;
using MediatR;

namespace BusinessExample.Core.Exchanges.LoanDecisions
{
    public class RaiseLoanDecisionReferredEvent : FlowActivityRequest<RaiseLoanDecisionReferredEvent.Response>
    {
        public static FlowActivityDefinition<RaiseLoanDecisionReferredEvent, Response> NewDefinition() =>
            new FlowActivityDefinition<RaiseLoanDecisionReferredEvent, Response>();

        [BoundValue, NotNullValue]
        public LoanApplication LoanApplication { get; set; }

        [BoundValue, NotNullValue]
        public LoanDecision LoanDecision { get; set; }

        public class Response
        {
        }
    }

    public class RaiseLoanDecisionReferredEventHandler 
        : IRequestHandler<RaiseLoanDecisionReferredEvent, RaiseLoanDecisionReferredEvent.Response>
    {
        private readonly ILoanEventService _loanEventService;

        public RaiseLoanDecisionReferredEventHandler(ILoanEventService loanEventService)
        {
            _loanEventService = loanEventService ?? throw new ArgumentNullException(nameof(loanEventService));
        }

        public async Task<RaiseLoanDecisionReferredEvent.Response> Handle(
            RaiseLoanDecisionReferredEvent request, CancellationToken cancellationToken)
        {
            await _loanEventService.RaiseEvent(LoanEventType.Referred, request.LoanApplication.Id, request.LoanDecision.Id);

            return new RaiseLoanDecisionReferredEvent.Response();
        }
    }

    public static class RaiseLoanDecisionReferredEventMocks
    {
        public static FlowContext MockRaiseLoanDecisionReferredEvent(this FlowContext flowContext, bool?[] isEventRaised = null)
        {
            flowContext.MockActivity<RaiseLoanDecisionReferredEvent, RaiseLoanDecisionReferredEvent.Response>(req =>
            {
                if (isEventRaised != null) isEventRaised[0] = true;
                return new RaiseLoanDecisionReferredEvent.Response();
            });

            return flowContext;
        }
    }
}