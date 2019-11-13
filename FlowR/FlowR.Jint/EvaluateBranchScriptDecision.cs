using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jint;

namespace FlowR.Jint
{
    public class EvaluateBranchScriptDecisionRequest : FlowDecisionRequest<string>
    {
        [BoundValue]
        public object SwitchValue { get; set; }
    }

    public class EvaluateBranchScriptDecision : FlowDecisionHandler<EvaluateBranchScriptDecisionRequest, string>
    {
        public override Task<int> Handle(EvaluateBranchScriptDecisionRequest request, CancellationToken cancellationToken)
        {
            var matchingBranchIndex = GetTrueBranchIndex(request.Branches, request.SwitchValue);

            return Task.FromResult(matchingBranchIndex);
        }

        private static int GetTrueBranchIndex(IEnumerable<FlowDecisionRequest<string>.Branch> branches, object switchValue)
        {
            var branchList = branches.ToList();

            var engine = new Engine()
                .SetValue("value", switchValue);

            var trueBranchIndexes =
                branchList
                    .Select((value, index) => new { Branch = value, Index = index })
                    .Where(pair => IsBranchTrue(pair.Branch, engine))
                    .Select(pair => pair.Index + 1)
                    .ToList();

            if (trueBranchIndexes.Count > 1)
            {
                throw new FlowException(
                    $"Found multiple true branches ({trueBranchIndexes.Count}) for switch value when at most one was expected");
            }

            if (trueBranchIndexes.Count == 1)
            {
                var matchingBranchIndex = trueBranchIndexes.First() - 1;
                return matchingBranchIndex;
            }

            var defaultBranchIndexes =
                branchList
                    .Select((value, index) => new { Branch = value, Index = index })
                    .Where(pair => pair.Branch.Targets == null)
                    .Select(pair => pair.Index + 1)
                    .ToList();

            var defaultBranchIndex = defaultBranchIndexes.First() - 1;
            return defaultBranchIndex;
        }

        private static bool IsBranchTrue(FlowDecisionRequest<string>.Branch branch, Engine engine)
        {
            var isBranchTrue =
                branch.Targets?
                    .Any(t => (bool)engine
                        .Execute(t)
                        .GetCompletionValue()
                        .ToObject());

            return isBranchTrue.GetValueOrDefault();
        }
    }
}
