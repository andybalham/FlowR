using BusinessExample.Core.Entities;
using BusinessExample.Core.Exchanges.Checks;
using BusinessExample.Core.Exchanges.Communications;
using BusinessExample.Core.Exchanges.LoanDecisions;
using FlowR;
using MediatR;

namespace BusinessExample.Core.Exchanges.LoanApplications
{
    public class MakeLoanApplicationDecision : FlowActivityRequest<MakeLoanApplicationDecision.Response>
    {
        [NotNullValue]
        public LoanApplication LoanApplication { get; set; }

        public class Response : FlowResponse
        {
            public LoanDecision LoanDecision { get; set; }
        }
    }

    public class MakeLoanApplicationDecisionHandler : BusinessFlowHandler<MakeLoanApplicationDecision, MakeLoanApplicationDecision.Response>
    {
        public MakeLoanApplicationDecisionHandler(IMediator mediator, IFlowLogger<MakeLoanApplicationDecisionHandler> logger) 
            : base(mediator, logger)
        {
        }

        protected override void OnDebugEvent(string stepName, FlowHandlerDebugEvent debugEvent, FlowValues flowValues)
        {
        }

        public override FlowDefinition GetFlowDefinition() =>
            new FlowDefinition()

                .Do("InitialiseNewDecision", InitialiseNewLoanDecision.NewDefinition())

                // ------------------------------------------------------------------------------------------------

                .Do("CheckEligibility", CheckEligibility.NewDefinition())
                .Check("IsEligible", FlowValueDecision<bool?>.NewDefinition()
                    .BindInput(rq => rq.SwitchValue, nameof(CheckEligibility.Response.IsEligible)))
                .When(false).Goto("SetResultToDecline")
                .When(true).Goto("CheckAffordability")
                .Else().Exception()

                // ------------------------------------------------------------------------------------------------

                .Do("CheckAffordability", CheckAffordability.NewDefinition())
                .Check("AffordabilityRating", FlowValueDecision<AffordabilityRating?>.NewDefinition()
                    .BindInput(rq => rq.SwitchValue, nameof(CheckAffordability.Response.AffordabilityRating)))
                .When(AffordabilityRating.Fair).Goto("SetResultToRefer")
                .When(AffordabilityRating.Poor).Goto("SetResultToDecline")
                .When(AffordabilityRating.Good).Goto("CheckIdentity")
                .Else().Exception()

                // ------------------------------------------------------------------------------------------------

                .Do("CheckIdentity", CheckIdentity.NewDefinition())
                .Check("IdentityCheckResult", FlowValueDecision<IdentityCheckResult?>.NewDefinition()
                    .BindInput(rq => rq.SwitchValue, nameof(CheckIdentity.Response.IdentityCheckResult)))
                .When(IdentityCheckResult.ServiceUnavailable).Goto("SetResultToRefer")
                .When(IdentityCheckResult.IdentityNotFound).Goto("SetResultToDecline")
                .When(IdentityCheckResult.IdentityFound).Goto("SetResultToAccept")
                .Else().Exception()

                // ------------------------------------------------------------------------------------------------

                .Do("SetResultToAccept", SetLoanDecisionResult.NewDefinition()
                    .SetValue(rq => rq.Result, LoanDecisionResult.Accept))
                .Goto("SaveDecision")

                .Do("SetResultToRefer", SetLoanDecisionResult.NewDefinition()
                    .SetValue(rq => rq.Result, LoanDecisionResult.Refer))
                .Goto("SaveDecision")

                .Do("SetResultToDecline", SetLoanDecisionResult.NewDefinition()
                    .SetValue(rq => rq.Result, LoanDecisionResult.Decline))
                .Goto("SaveDecision")

                // ------------------------------------------------------------------------------------------------

                .Do("SaveDecision", CreateLoanDecision.NewDefinition())

                .Check("LoanDecisionResult", FlowValueDecision<LoanDecisionResult?>.NewDefinition()
                    .BindInput(rq => rq.SwitchValue, nameof(SetLoanDecisionResult.Response.Result)))
                .When(LoanDecisionResult.Decline).Goto("PostDeclineActions")
                .When(LoanDecisionResult.Refer).Goto("PostReferActions")
                .When(LoanDecisionResult.Accept).Goto("PostAcceptActions")
                .Else().Exception()

                // ------------------------------------------------------------------------------------------------

                .Label("PostAcceptActions")
                .Do("SendAcceptConfirmationEmail", SendEmail.NewDefinition()
                    .SetValue(rq => rq.TemplateName, "AcceptConfirmation")
                    .BindInput(rq => rq.EmailAddress, 
                        nameof(MakeLoanApplicationDecision.LoanApplication), (LoanApplication la) => la.EmailAddress)
                    .BindInputs(rq => rq.DataObjects,
                        nameof(MakeLoanApplicationDecision.LoanApplication), nameof(InitialiseNewLoanDecision.Response.LoanDecision))
                    .BindInput(rq => rq.ParentId, 
                        nameof(InitialiseNewLoanDecision.Response.LoanDecision), (LoanDecision ld) => ld.Id))
                .End()

                // ------------------------------------------------------------------------------------------------

                .Label("PostReferActions")
                .Do("SendReferNotificationEmail", SendEmail.NewDefinition()
                    .SetValue(rq => rq.TemplateName, "ReferNotification")
                    .BindInput(rq => rq.EmailAddress, 
                        nameof(MakeLoanApplicationDecision.LoanApplication), (LoanApplication la) => la.EmailAddress)
                    .BindInputs(rq => rq.DataObjects, 
                        nameof(MakeLoanApplicationDecision.LoanApplication), nameof(InitialiseNewLoanDecision.Response.LoanDecision))
                    .BindInput(rq => rq.ParentId, 
                        nameof(InitialiseNewLoanDecision.Response.LoanDecision), (LoanDecision ld) => ld.Id))
                .Do("RaiseLoanReferredEvent", RaiseLoanDecisionReferredEvent.NewDefinition())
                .End()

                // ------------------------------------------------------------------------------------------------

                .Label("PostDeclineActions")
                .Do("SendDeclineConfirmationEmail", SendEmail.NewDefinition()
                    .SetValue(rq => rq.TemplateName, "DeclineConfirmation")
                    .BindInput(rq => rq.EmailAddress, 
                        nameof(MakeLoanApplicationDecision.LoanApplication), (LoanApplication la) => la.EmailAddress)
                    .BindInputs(rq => rq.DataObjects,
                        nameof(MakeLoanApplicationDecision.LoanApplication), nameof(InitialiseNewLoanDecision.Response.LoanDecision))
                    .BindInput(rq => rq.ParentId, 
                        nameof(InitialiseNewLoanDecision.Response.LoanDecision), (LoanDecision ld) => ld.Id))
                .End();
    }
}
