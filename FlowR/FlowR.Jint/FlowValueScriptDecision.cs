using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jint;

namespace FlowR.Jint
{
    public class FlowValueScriptDecisionRequest<TSwitch> : FlowDecisionRequest<TSwitch>
    {
        [NotNullValue]
        public string SwitchValueScript { get; set; }

        [BoundValue]
        public object FlowValue { get; set; }
    }

    public class FlowValueScriptDecision<TSwitch> : FlowDecisionHandler<FlowValueScriptDecisionRequest<TSwitch>, TSwitch>
    {
        public override Task<int> Handle(FlowValueScriptDecisionRequest<TSwitch> request, CancellationToken cancellationToken)
        {
            var switchValue =
                (TSwitch)new Engine()
                    .SetValue("value", request.FlowValue)
                    .Execute(request.SwitchValueScript)
                    .GetCompletionValue()
                    .ToObject();

            var matchingBranchIndex = GetMatchingBranchIndex(switchValue, request.Branches, BranchTargetsContains);

            return Task.FromResult(matchingBranchIndex);
        }
    }
}
