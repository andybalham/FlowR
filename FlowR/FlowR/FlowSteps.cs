using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowR
{
    public abstract class FlowStep
    {
        public string Name { get; set; }

        public string Text { get; set; }

        public int Index { get; set; }

        public FlowStepDefinition Definition { get; set; }

        public FlowOverrideKey OverrideKey { get; set; }
    }

    public class ActivityFlowStep : FlowStep
    {
    }

    public class LabelFlowStep : FlowStep
    {
    }

    public class GotoFlowStep : FlowStep
    {
        public string NextStepName { get; set; }
    }

    public class EndFlowStep : FlowStep
    {
    }

    public abstract class DecisionFlowStepBase : FlowStep
    {
        public class Branch
        {
            public IEnumerable<object> Targets { get; set; }
            public string NextStepName { get; set; }
            public bool IsEnd { get; set; }
            public bool IsUnhandled { get; set; }
        }

        internal List<Branch> Branches { get; } = new List<Branch>();
    }

    public class DecisionFlowStep<TSwitch, TFlowRequest, TFlowResponse> : DecisionFlowStepBase
        where TFlowRequest : FlowActivityRequest<TFlowResponse>
    {
        private readonly FlowDefinition<TFlowRequest, TFlowResponse> _flowDefinition;

        public DecisionFlowStep(FlowDefinition<TFlowRequest, TFlowResponse> flowDefinition)
        {
            _flowDefinition = flowDefinition;
        }

        public DecisionFlowStepCriteria<TSwitch> When(params TSwitch[] targets)
        {
            var criteria = Array.ConvertAll(targets, t => (object)t);

            return new DecisionFlowStepCriteria<TSwitch>(this, this.Branches, criteria);
        }

        public class DecisionFlowStepCriteria<T>
        {
            private readonly DecisionFlowStep<T, TFlowRequest, TFlowResponse> _flowStep;
            private readonly IList<Branch> _branches;
            private readonly IEnumerable<object> _criteria;

            public DecisionFlowStepCriteria(DecisionFlowStep<T, TFlowRequest, TFlowResponse> flowStep, IList<Branch> branches, IEnumerable<object> criteria)
            {
                _flowStep = flowStep;
                _branches = branches;
                _criteria = criteria;
            }

            public DecisionFlowStep<T, TFlowRequest, TFlowResponse> Goto(string nextStepName)
            {
                _branches.Add(new Branch { Targets = _criteria, NextStepName = nextStepName });
                return _flowStep;
            }

            public DecisionFlowStep<T, TFlowRequest, TFlowResponse> End()
            {
                _branches.Add(new Branch { Targets = _criteria, IsEnd = true });
                return _flowStep;
            }
        }

        public DecisionStepElse Else()
        {
            return new DecisionStepElse(this.Branches, _flowDefinition);
        }

        public class DecisionStepElse
        {
            private readonly IList<Branch> _branches;
            private readonly FlowDefinition<TFlowRequest, TFlowResponse> _flowDefinition;

            public DecisionStepElse(IList<Branch> branches, FlowDefinition<TFlowRequest, TFlowResponse> flowDefinition)
            {
                _branches = branches;
                _flowDefinition = flowDefinition;
            }
            public FlowDefinition<TFlowRequest, TFlowResponse> Goto(string nextStepName)
            {
                _branches.Add(new Branch { NextStepName = nextStepName });
                return _flowDefinition;
            }

            public FlowDefinition<TFlowRequest, TFlowResponse> Continue()
            {
                _branches.Add(new Branch());
                return _flowDefinition;
            }

            public FlowDefinition<TFlowRequest, TFlowResponse> End()
            {
                _branches.Add(new Branch { IsEnd = true });
                return _flowDefinition;
            }

            public FlowDefinition<TFlowRequest, TFlowResponse> Unhandled()
            {
                _branches.Add(new Branch { IsUnhandled = true });
                return _flowDefinition;
            }
        }
    }
}
