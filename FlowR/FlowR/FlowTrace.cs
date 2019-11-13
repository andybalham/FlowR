using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowR
{
    public class FlowTrace
    {
        private readonly List<FlowTraceStep> _steps = new List<FlowTraceStep>();

        public override string ToString()
        {
            var toString = string.Join(" -> ", _steps.ConvertAll(s => s.ToString()).ToArray());
            return toString;
        }

        internal void AddStep(FlowTraceStep step)
        {
            _steps.Add(step);
        }

        internal void AddSubFlowTrace(FlowTrace subFlowTrace)
        {
            _steps.Last().SubFlowTrace = subFlowTrace;
        }
    }

    public class FlowTraceStep
    {
        public FlowTraceStepType StepType { get; set; }
        public string Name { get; set; }
        public IEnumerable<object> BranchTargets { get; set; }
        public FlowTrace SubFlowTrace { get; set; }

        public override string ToString()
        {
            string toString;

            switch (this.StepType)
            {
                case FlowTraceStepType.Decision:
                    toString = $"{this.Name} ? {string.Join("|", this.BranchTargets?.ToArray() ?? new object[] { "ELSE" })}";
                    break;

                case FlowTraceStepType.Label:
                    toString = $"#{this.Name}";
                    break;

                case FlowTraceStepType.Event:
                    toString = $"!{this.Name}";
                    break;

                default:
                    toString = this.Name;
                    if (this.SubFlowTrace != null) toString += $" [ {this.SubFlowTrace} ]";
                    break;
            }
            return toString;
        }
    }

    public enum FlowTraceStepType
    {
        Unknown = 0,
        Activity,
        Decision,
        Label,
        Event
    }
}