using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jint;

namespace FlowR.Jint
{
    public class FlowValueScriptDecisionRequest<TSwitch> : FlowDecision<TSwitch>
    {
        [NotNullValue]
        public string SwitchValueScript { get; set; }

        public object FlowValue { get; set; }

        public override int GetMatchingBranchIndex()
        {
            var switchValue =
                (TSwitch)new Engine()
                    .SetValue("value", this.FlowValue)
                    .Execute(this.SwitchValueScript)
                    .GetCompletionValue()
                    .ToObject();

            var matchingBranchIndex = GetMatchingBranchIndex(switchValue, this.Branches, BranchTargetsContains);

            return matchingBranchIndex;
        }
    }
}
