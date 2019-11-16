using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlowR
{
    public class FlowValueDecision<TSwitch> : FlowDecision<TSwitch>
    {
        [BoundValue, NotNullValue]
        public virtual TSwitch SwitchValue { get; set; }

        public override int GetMatchingBranchIndex()
        {
            var matchingBranchIndex = GetMatchingBranchIndex(this.SwitchValue, this.Branches, BranchTargetsContains);
            return matchingBranchIndex;
        }

        public static FlowDecisionDefinition<FlowValueDecision<TSwitch>, TSwitch> NewDefinition()
        {
            return new FlowDecisionDefinition<FlowValueDecision<TSwitch>, TSwitch>();
        }
    }

    public class NullableFlowValueDecision<TSwitch> : FlowValueDecision<TSwitch>
    {
        [BoundValue]
        public override TSwitch SwitchValue { get; set; }
    }
}
