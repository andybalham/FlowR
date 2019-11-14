using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace FlowR
{
    public abstract class FlowDecisionBase : IFlowStepRequest, IRequest<int>
    {
        public FlowContext FlowContext { get; set; }

        public virtual string GetText() => null;

        public abstract void AddBranch(IEnumerable<object> targets, string destination, bool isEnd);
        
        public abstract int GetMatchingBranchIndex();
    }

    public abstract class FlowDecision<TSwitch> : FlowDecisionBase
    {
        private readonly IList<Branch> _branches = new List<Branch>();

        public IEnumerable<Branch> Branches => _branches;

        public class Branch
        {
            public IEnumerable<TSwitch> Targets { get; internal set; }
            public string Destination { get; internal set; }
            public bool IsEnd { get; set; }
        }

        public override void AddBranch(IEnumerable<object> targets, string destination, bool isEnd)
        {
            // TODO: How can we use converters if the definition targets are strings?

            var switchTargets = targets?.ToList().ConvertAll(t => (TSwitch)t);

            var branch = new Branch { Targets = switchTargets, Destination = destination, IsEnd = isEnd };

            _branches.Add(branch);
        }

        protected int GetMatchingBranchIndex(TSwitch switchValue, IEnumerable<Branch> branches, Func<Branch, TSwitch, bool> isMatch)
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

        protected virtual bool BranchTargetsContains(Branch branch, TSwitch switchValue)
        {
            var isMatch = branch.Targets?.Contains(switchValue);
            return isMatch.GetValueOrDefault();
        }
    }
}
