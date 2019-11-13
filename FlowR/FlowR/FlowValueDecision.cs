using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlowR
{
    public class FlowValueDecision<TSwitch> : FlowDecisionRequest<TSwitch>
    {
        [BoundValue, NotNullValue]
        public virtual TSwitch SwitchValue { get; set; }
    }

    public class NullableFlowValueDecision<TSwitch> : FlowValueDecision<TSwitch>
    {
        [BoundValue]
        public override TSwitch SwitchValue { get; set; }
    }

    public abstract class FlowValueDecisionHandler<TReq, TSwitch> : FlowDecisionHandler<TReq, TSwitch>
        where TReq : FlowValueDecision<TSwitch>
    {
        public override Task<int> Handle(TReq request, CancellationToken cancellationToken)
        {
            var matchingBranchIndex = GetMatchingBranchIndex(request.SwitchValue, request.Branches, BranchTargetsContains);
            return Task.FromResult(matchingBranchIndex);
        }
    }
}
