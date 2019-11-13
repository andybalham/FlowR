using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlowR.StepLibrary.Activities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowR.Tests.Domain.FlowTests
{
    public class DecisionBindingAttributesFlowRequest : FlowActivityRequest<DecisionBindingAttributesFlowResponse>
    {
        public bool FlowInput1 { get; set; }
        public bool FlowInput2 { get; set; }
    }

    public class DecisionBindingAttributesFlowResponse : FlowResponse
    {
        public bool FlowOutput { get; set; }
    }

    public class DecisionBindingAttributesFlow : FlowHandler<DecisionBindingAttributesFlowRequest, DecisionBindingAttributesFlowResponse>
    {
        public DecisionBindingAttributesFlow(IMediator mediator, IFlowLogger<DecisionBindingAttributesFlow> logger) : base(mediator, logger)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            var isDecisionValueTrue = new FlowDecisionDefinition<BindingAttributesDecisionRequest, bool>()
                .SetValue(r => r.DecisionInput1Name, nameof(DecisionBindingAttributesFlowRequest.FlowInput1))
                .SetValue(r => r.NamedDecisionInputName, nameof(DecisionBindingAttributesFlowRequest.FlowInput2));

            var setFlowOutputToFalse = new FlowActivityDefinition<SetBoolFlowValueRequest, SetBoolFlowValueResponse>()
                .SetValue(r => r.OutputValue, false)
                .BindOutput(r => r.Output, nameof(DecisionBindingAttributesFlowResponse.FlowOutput));

            var setFlowOutputToTrue = new FlowActivityDefinition<SetBoolFlowValueRequest, SetBoolFlowValueResponse>()
                .SetValue(r => r.OutputValue, true)
                .BindOutput(r => r.Output, nameof(DecisionBindingAttributesFlowResponse.FlowOutput));

            return new FlowDefinition()
                .Check("Is_decision_value_true", isDecisionValueTrue)
                .When(true).Goto("Set_flow_output_to_true")
                .Else().Continue()

                .Do("Set_flow_output_to_false", setFlowOutputToFalse)
                .End()

                .Do("Set_flow_output_to_true", setFlowOutputToTrue);
        }
    }

    public class BindingAttributesDecisionRequest : FlowDecisionRequest<bool>
    {
        [InputBindingName]
        public string DecisionInput1Name { get; set; }

        [InputBindingName("DecisionInput2")]
        public string NamedDecisionInputName { get; set; }

        [BoundValue]
        public bool DecisionInput1 { get; set; }
        [BoundValue]
        public bool DecisionInput2 { get; set; }
    }

    public class BindingAttributesDecision : FlowDecisionHandler<BindingAttributesDecisionRequest, bool>
    {
        public override Task<int> Handle(BindingAttributesDecisionRequest request, CancellationToken cancellationToken)
        {
            var switchValue = request.DecisionInput1 && request.DecisionInput2;
            var matchingBranchIndex = GetMatchingBranchIndex(switchValue, request.Branches, BranchTargetsContains);
            return Task.FromResult(matchingBranchIndex);
        }
    }
}
