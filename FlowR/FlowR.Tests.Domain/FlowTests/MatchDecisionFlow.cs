using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlowR.StepLibrary.Activities;
using FlowR.StepLibrary.Decisions;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class MatchDecisionFlowRequest : FlowActivityRequest<MatchDecisionFlowResponse>
    {
        public int IntValue { get; set; }
        public string StringValue { get; set; }
    }

    public class MatchDecisionFlowResponse : FlowResponse
    {
        public string BranchValue { get; set; }
    }

    public class MatchDecisionFlow : FlowHandler<MatchDecisionFlowRequest, MatchDecisionFlowResponse>
    {
        public MatchDecisionFlow(IMediator mediator, IFlowLogger<MatchDecisionFlow> logger) : base(mediator, logger)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            #region Definitions

            var intValueDecision = new FlowDecisionDefinition<
                    IntFlowValueDecision, int?>()
                .BindInput(rq => rq.SwitchValue, nameof(MatchDecisionFlowRequest.IntValue));

            var stringValueDecision = new FlowDecisionDefinition<StringFlowValueDecision, string>()
                .BindInput(rq => rq.SwitchValue, nameof(MatchDecisionFlowRequest.StringValue));

            var setOutputToX = new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                .SetValue(rq => rq.OutputValue, "X")
                .BindOutput(rs => rs.Output, nameof(MatchDecisionFlowResponse.BranchValue));

            var setOutputToY = new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                .SetValue(rq => rq.OutputValue, "Y")
                .BindOutput(rs => rs.Output, nameof(MatchDecisionFlowResponse.BranchValue));

            var setOutputToZa = new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                .SetValue(rq => rq.OutputValue, "ZA")
                .BindOutput(rs => rs.Output, nameof(MatchDecisionFlowResponse.BranchValue));

            var setOutputToZz = new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                .SetValue(rq => rq.OutputValue, "ZZ")
                .BindOutput(rs => rs.Output, nameof(MatchDecisionFlowResponse.BranchValue));

            #endregion

            return new FlowDefinition()
                .Check("Int_value", intValueDecision)
                .When(1).Goto("Set_output_to_X")
                .When(2, 3).Goto("Set_output_to_Y")
                .Else().Goto("String_value")
                
                .Do("Set_output_to_X", setOutputToX)
                .End()

                .Do("Set_output_to_Y", setOutputToY)
                .End()

                .Check("String_value", stringValueDecision)
                .When("A").Goto("Set_output_to_ZA")
                .Else().Continue()

                .Do("Set_output_to_ZZ", setOutputToZz)
                .End()

                .Do("Set_output_to_ZA", setOutputToZa);
        }
    }
}
