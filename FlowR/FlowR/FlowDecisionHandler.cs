using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlowR
{
    public abstract class FlowDecisionHandler<TReq, TSwitch> : IFlowDecisionRequestHandler<TReq, TSwitch>
        where TReq : FlowDecisionRequest<TSwitch>
    {
        public abstract Task<int> Handle(TReq request, CancellationToken cancellationToken);

        protected int GetMatchingBranchIndex(TSwitch switchValue, IEnumerable<FlowDecisionRequest<TSwitch>.Branch> branches,
            Func<FlowDecisionRequest<TSwitch>.Branch, TSwitch, bool> isMatch)
        {
            var branchList = branches.ToList();

            var matchingBranchIndexes =
                branchList
                    .Select((value, index) => new { Branch = value, Index = index })
                    .Where(pair => isMatch(pair.Branch, switchValue))
                    .Select(pair => pair.Index + 1)
                    .ToList();

            if (matchingBranchIndexes.Count > 1)
            {
                throw new FlowException(
                    $"Found multiple matching branches ({matchingBranchIndexes.Count}) for switch value " +
                    $"'{switchValue}' when at most one was expected");
            }

            if (matchingBranchIndexes.Count == 1)
            {
                var matchingBranchIndex = matchingBranchIndexes.FirstOrDefault() - 1;
                return matchingBranchIndex;
            }

            var defaultBranchIndexes =
                branchList
                    .Select((value, index) => new { Branch = value, Index = index })
                    .Where(pair => pair.Branch.Targets == null)
                    .Select(pair => pair.Index + 1)
                    .ToList();

            var defaultBranchIndex = defaultBranchIndexes.FirstOrDefault() - 1;
            return defaultBranchIndex;
        }

        protected virtual bool BranchTargetsContains(FlowDecisionRequest<TSwitch>.Branch branch, TSwitch switchValue)
        {
            var isMatch = branch.Targets?.Contains(switchValue);
            return isMatch.GetValueOrDefault();
        }
    }
}
