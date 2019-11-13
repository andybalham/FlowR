using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace FlowR
{
    public abstract class FlowDecisionRequestBase : IFlowStepRequest, IRequest<int>
    {
        public FlowContext FlowContext { get; set; }

        public virtual string GetText() => null;

        public abstract void AddBranch(IEnumerable<object> targets, string destination, bool isEnd);
    }

    public abstract class FlowDecisionRequest<TSwitch> : FlowDecisionRequestBase
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
    }
}
