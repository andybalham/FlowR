using BusinessExample.Core.Entities;
using BusinessExample.Core.Exchanges.Checks;
using BusinessExample.Core.Exchanges.Communications;
using BusinessExample.Core.Exchanges.LoanDecisions;
using FlowR;
using FlowR.StepLibrary.Decisions;
using MediatR;

namespace BusinessExample.Core.Exchanges.LoanApplications
{
    // TODO: Think about changing the name of this, perhaps to PerformLoanDecisionProcessRequest
    // TODO: Should this be in the LoanApplication namespace? It is an operation on a LoanApplication after all
    public class MakeLoanApplicationDecision : FlowActivityRequest<MakeLoanApplicationDecision.Response>
    {
        [BoundValue, NotNullValue]
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

        public override FlowDefinition GetFlowDefinition() =>
            new FlowDefinition()

                .Do("InitialiseNewDecision",
                    new FlowActivityDefinition<InitialiseNewLoanDecision, InitialiseNewLoanDecision.Response>())

                // ------------------------------------------------------------------------------------------------

                .Do("CheckEligibility",
                    new FlowActivityDefinition<CheckEligibility, CheckEligibility.Response>())
                .Check("IsEligible",
                    new FlowDecisionDefinition<BoolFlowValueDecision, bool?>()
                        .BindInput(rq => rq.SwitchValue, nameof(CheckEligibility.Response.IsEligible)))
                .When(false).Goto("SetResultToDecline")
                .Else().Continue()

                // ------------------------------------------------------------------------------------------------

                .Do("CheckAffordability",
                    new FlowActivityDefinition<CheckAffordability, CheckAffordability.Response>())
                .Check("AffordabilityRating",
                    new FlowDecisionDefinition<AffordabilityRatingDecision, AffordabilityRating?>()
                        .BindInput(rq => rq.SwitchValue, nameof(CheckAffordability.Response.AffordabilityRating)))
                .When(AffordabilityRating.Fair).Goto("SetResultToRefer")
                .When(AffordabilityRating.Poor).Goto("SetResultToDecline")
                .Else().Continue()

                // ------------------------------------------------------------------------------------------------

                .Do("CheckIdentity",
                    new FlowActivityDefinition<CheckIdentity, CheckIdentity.Response>())
                .Check("IdentityCheckResult",
                    new FlowDecisionDefinition<IdentityCheckResultDecision, IdentityCheckResult?>()
                        .BindInput(rq => rq.SwitchValue, nameof(CheckIdentity.Response.IdentityCheckResult)))
                .When(IdentityCheckResult.ServiceUnavailable).Goto("SetResultToRefer")
                .When(IdentityCheckResult.IdentityNotFound).Goto("SetResultToDecline")
                .Else().Continue()

                // ------------------------------------------------------------------------------------------------

                .Do("SetResultToAccept", 
                    new FlowActivityDefinition<SetLoanDecisionResult, SetLoanDecisionResult.Response>()
                        .SetValue(rq => rq.Result, LoanDecisionResult.Accept))
                .Goto("SaveDecision")

                .Do("SetResultToRefer",
                    new FlowActivityDefinition<SetLoanDecisionResult, SetLoanDecisionResult.Response>()
                        .SetValue(rq => rq.Result, LoanDecisionResult.Refer))
                .Goto("SaveDecision")

                .Do("SetResultToDecline",
                    new FlowActivityDefinition<SetLoanDecisionResult, SetLoanDecisionResult.Response>()
                        .SetValue(rq => rq.Result, LoanDecisionResult.Decline))
                .Goto("SaveDecision")

                // ------------------------------------------------------------------------------------------------

                .Do("SaveDecision",
                    new FlowActivityDefinition<CreateLoanDecision, CreateLoanDecision.Response>())

                .Check("LoanDecisionResult", 
                    new FlowDecisionDefinition<LoanDecisionResultDecision, LoanDecisionResult?>()
                        .BindInput(rq => rq.SwitchValue, nameof(SetLoanDecisionResult.Response.Result)))
                .When(LoanDecisionResult.Decline).Goto("PostDeclineActions")
                .When(LoanDecisionResult.Refer).Goto("PostReferActions")
                .Else().Continue()

                // ------------------------------------------------------------------------------------------------

                .Label("PostAcceptActions")
                .Do("SendAcceptConfirmationEmail",
                    new FlowActivityDefinition<SendEmail, SendEmail.Response>()
                        .SetValue(rq => rq.TemplateName, "AcceptConfirmation")
                        .BindInput(rq => rq.EmailAddress, nameof(MakeLoanApplicationDecision.LoanApplication), 
                            (LoanApplication la) => la.EmailAddress)
                        .BindInputs(rq => rq.DataObjects,
                            nameof(MakeLoanApplicationDecision.LoanApplication), nameof(InitialiseNewLoanDecision.Response.LoanDecision))
                        .BindInput(rq => rq.ParentId, nameof(InitialiseNewLoanDecision.Response.LoanDecision),
                            (LoanDecision ld) => ld.Id))
                .End()

                // ------------------------------------------------------------------------------------------------

                .Label("PostReferActions")
                .Do("SendReferNotificationEmail",
                    new FlowActivityDefinition<SendEmail, SendEmail.Response>()
                        .SetValue(rq => rq.TemplateName, "ReferNotification")
                        .BindInput(rq => rq.EmailAddress, nameof(MakeLoanApplicationDecision.LoanApplication),
                            (LoanApplication la) => la.EmailAddress)
                        .BindInputs(rq => rq.DataObjects, 
                            nameof(MakeLoanApplicationDecision.LoanApplication), nameof(InitialiseNewLoanDecision.Response.LoanDecision))
                        .BindInput(rq => rq.ParentId, nameof(InitialiseNewLoanDecision.Response.LoanDecision),
                            (LoanDecision ld) => ld.Id))
                .Do("RaiseLoanReferredEvent",
                    new FlowActivityDefinition<RaiseLoanDecisionReferredEvent, RaiseLoanDecisionReferredEvent.Response>())
                .End()

                // ------------------------------------------------------------------------------------------------

                .Label("PostDeclineActions")
                .Do("SendDeclineConfirmationEmail",
                    new FlowActivityDefinition<SendEmail, SendEmail.Response>()
                        .SetValue(rq => rq.TemplateName, "DeclineConfirmation")
                        .BindInput(rq => rq.EmailAddress, nameof(MakeLoanApplicationDecision.LoanApplication),
                            (LoanApplication la) => la.EmailAddress)
                        .BindInputs(rq => rq.DataObjects,
                            nameof(MakeLoanApplicationDecision.LoanApplication), nameof(InitialiseNewLoanDecision.Response.LoanDecision))
                        .BindInput(rq => rq.ParentId, nameof(InitialiseNewLoanDecision.Response.LoanDecision),
                            (LoanDecision ld) => ld.Id))
                .End();
    }
}
