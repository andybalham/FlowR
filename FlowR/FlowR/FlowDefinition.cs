using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowR
{
    public class FlowDefinition
    {
        public string Criteria { get; }

        #region Member declarations

        private readonly List<FlowStep> _steps = new List<FlowStep>();

        #endregion

        #region Properties

        public IReadOnlyList<FlowStep> Steps => _steps;

        private ISet<string> StepNames { get; } = new HashSet<string>();

        #endregion

        #region Constructors

        public FlowDefinition() : this(null)
        {
        }

        public FlowDefinition(string criteria)
        {
            Criteria = criteria;
        }

        #endregion

        #region Builder methods

        public DecisionFlowStep<TSwitch> Check<TReq, TSwitch>(string stepName, FlowDecisionDefinition<TReq, TSwitch> definition)
            where TReq : FlowDecision<TSwitch>
        {
            return Check(stepName, stepText: null, flowOverrideKey: null, definition);
        }

        public DecisionFlowStep<TSwitch> Check<TReq, TSwitch>(string stepName, string stepText, 
            FlowDecisionDefinition<TReq, TSwitch> definition) where TReq : FlowDecision<TSwitch>
        {
            return Check(stepName, stepText, flowOverrideKey: null, definition);
        }

        public DecisionFlowStep<TSwitch> Check<TReq, TSwitch>(string stepName, FlowOverrideKey flowOverrideKey, 
            FlowDecisionDefinition<TReq, TSwitch> definition) where TReq : FlowDecision<TSwitch>
        {
            return Check(stepName, stepText: null, flowOverrideKey, definition);
        }

        public DecisionFlowStep<TSwitch> Check<TReq, TSwitch>(string stepName, string stepText, FlowOverrideKey flowOverrideKey,
            FlowDecisionDefinition<TReq, TSwitch> definition) where TReq : FlowDecision<TSwitch>
        {
            var decisionFlowStep =
                new DecisionFlowStep<TSwitch>(this)
                {
                    Name = stepName, Definition = definition, Text = stepText, OverrideKey = flowOverrideKey
                };

            AddStep(decisionFlowStep);

            return decisionFlowStep;
        }

        public FlowDefinition Do<TReq, TRes>(string stepName, FlowActivityDefinition<TReq, TRes> definition)
            where TReq : FlowActivityRequest<TRes>
        {
            return Do(stepName, stepText: null, flowOverrideKey: null, definition);
        }

        public FlowDefinition Do<TReq, TRes>(string stepName, FlowOverrideKey flowOverrideKey, 
            FlowActivityDefinition<TReq, TRes> definition) where TReq : FlowActivityRequest<TRes>
        {
            return Do(stepName, stepText: null, flowOverrideKey, definition);
        }

        public FlowDefinition Do<TReq, TRes>(string stepName, string stepText,
            FlowActivityDefinition<TReq, TRes> definition) where TReq : FlowActivityRequest<TRes>
        {
            return Do(stepName, stepText, flowOverrideKey: null, definition);
        }

        public FlowDefinition Do<TReq, TRes>(string stepName, string stepText, FlowOverrideKey flowOverrideKey, 
            FlowActivityDefinition<TReq, TRes> definition) where TReq : FlowActivityRequest<TRes>
        {
            AddStep(new ActivityFlowStep
            {
                Name = stepName, Definition = definition, Text = stepText, OverrideKey = flowOverrideKey
            });

            return this;
        }

        public FlowDefinition End()
        {
            AddStep(new EndFlowStep());
            return this;
        }

        public FlowDefinition Label(string stepName)
        {
            AddStep(new LabelFlowStep { Name = stepName, Index = this.Steps.Count });
            return this;
        }

        public FlowDefinition Label(string stepName, string stepText)
        {
            AddStep(new LabelFlowStep { Name = stepName, Text = stepText });
            return this;
        }

        public FlowDefinition Goto(string nextStepName)
        {
            AddStep(new GotoFlowStep { NextStepName = nextStepName });
            return this;
        }

        #endregion

        #region Public methods

        public IEnumerable<string> Validate()
        {
            var validationMessages = new List<string>();

            ValidateStepFlow(validationMessages);

            ValidateSettersAndBindings(validationMessages);

            return validationMessages;
        }

        public int GetStepIndex(string stepName)
        {
            var flowSteps = _steps.Where(s => string.Equals(s.Name, stepName)).ToList();

            if (flowSteps.Count != 1)
            {
                throw new FlowException(
                    $"Found {flowSteps.Count} matches for step '{stepName}' when 1 is expected");
            }

            var stepIndex = flowSteps.First().Index;

            return stepIndex;
        }

        #endregion

        #region Private methods

        private void ValidateSettersAndBindings(ICollection<string> validationMessages)
        {
            foreach (var flowStep in this.Steps)
            {
                if (flowStep.Definition == null)
                {
                    continue;
                }

                var duplicateSetterNames =
                    flowStep.Definition.Setters
                        .GroupBy(s => s.Item1.Name)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key);

                duplicateSetterNames.ToList().ForEach(n =>
                    validationMessages.Add($"Step '{flowStep.Name}' has multiple setters defined for property {n}"));

                var duplicateInputNames =
                    flowStep.Definition.Inputs
                        .GroupBy(s => s.Property.Name)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key);

                duplicateInputNames.ToList().ForEach(n =>
                    validationMessages.Add($"Step '{flowStep.Name}' has multiple inputs defined for property {n}"));

                var duplicateOutputNames =
                    flowStep.Definition.Outputs
                        .GroupBy(s => s.Property.Name)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key);

                duplicateOutputNames.ToList().ForEach(n =>
                    validationMessages.Add($"Step '{flowStep.Name}' has multiple outputs defined for property {n}"));
            }
        }

        private void ValidateStepFlow(List<string> validationMessages)
        {
            if (this.Steps.Count == 0)
            {
                return;
            }

            ValidateStepGotoTargets(validationMessages);

            if (validationMessages.Count > 0)
            {
                return;
            }

            ValidateStepCoverage(validationMessages);

            ValidateStepPath(validationMessages);
        }

        private void ValidateStepGotoTargets(List<string> validationMessages)
        {
            foreach (var flowStep in this.Steps)
            {
                switch (flowStep)
                {
                    case DecisionFlowStepBase decisionFlowStep:
                        validationMessages.AddRange(
                            from branch in decisionFlowStep.Branches 
                            where !string.IsNullOrEmpty(branch.NextStepName) && !this.StepNames.Contains(branch.NextStepName) 
                            select $"Step '{decisionFlowStep.Name}' branch target '{branch.NextStepName}' does not exist");
                        break;

                    case GotoFlowStep gotoFlowStep:
                        if (!this.StepNames.Contains(gotoFlowStep.NextStepName))
                        {
                            validationMessages.Add($"Goto target '{gotoFlowStep.NextStepName}' does not exist");
                        }
                        break;
                }
            }
        }

        private void ValidateStepPath(ICollection<string> validationMessages)
        {
            for (var stepIndex = 0; stepIndex < this.Steps.Count; stepIndex++)
            {
                if (CanGetToEnd(stepIndex, new HashSet<string>()))
                {
                    continue;
                }

                var stepName = this.Steps[stepIndex].Name;

                if (!string.IsNullOrEmpty(stepName))
                {
                    validationMessages.Add($"'{stepName}' is in a closed loop");
                }
            }
        }

        private bool CanGetToEnd(int stepIndex, ISet<string> visitedStepNames)
        {
            // TODO: Look at merging this with the VisitStep functionality

            if (stepIndex >= this.Steps.Count)
            {
                return true;
            }

            var flowStep = this.Steps[stepIndex];

            switch (flowStep)
            {
                case ActivityFlowStep _:
                case LabelFlowStep _:
                    if (visitedStepNames.Contains(flowStep.Name))
                    {
                        return false;
                    }

                    visitedStepNames.Add(flowStep.Name);

                    var nextStepIndex = stepIndex + 1;
                    return CanGetToEnd(nextStepIndex, visitedStepNames);

                case DecisionFlowStepBase decisionFlowStep:
                    if (visitedStepNames.Contains(flowStep.Name))
                    {
                        return false;
                    }

                    visitedStepNames.Add(flowStep.Name);

                    foreach (var branch in decisionFlowStep.Branches)
                    {
                        var branchStepIndex =
                            string.IsNullOrEmpty(branch.NextStepName)
                                ? stepIndex + 1
                                : GetStepIndex(branch.NextStepName);

                        var canBranchGetToEnd = CanGetToEnd(branchStepIndex, new HashSet<string>(visitedStepNames));

                        if (canBranchGetToEnd)
                        {
                            return true;
                        }
                    }
                    return false;

                case GotoFlowStep gotoFlowStep:
                    var gotoStepIndex = GetStepIndex(gotoFlowStep.NextStepName);
                    return CanGetToEnd(gotoStepIndex, visitedStepNames);

                case EndFlowStep _:
                    return true;

                default:
                    throw new FlowException($"The type of flow step '{flowStep.Name}' was not handled: {flowStep.GetType().FullName}");
            }
        }

        private void AddStep(FlowStep flowStep)
        {
            if (!string.IsNullOrEmpty(flowStep.Name))
            {
                if (this.StepNames.Contains(flowStep.Name))
                {
                    throw new FlowException($"Duplicate step name '{flowStep.Name}' specified in the steps");
                }

                this.StepNames.Add(flowStep.Name);
            }

            flowStep.Index = _steps.Count;
            _steps.Add(flowStep);
        }

        private void ValidateStepCoverage(ICollection<string> validationMessages)
        {
            var visitedStepNames = new HashSet<string>();
            VisitStep(0, visitedStepNames);

            var orphanedStepNames = new HashSet<string>(this.StepNames);
            orphanedStepNames.ExceptWith(visitedStepNames);

            foreach (var orphanedStepName in orphanedStepNames)
            {
                validationMessages.Add($"'{orphanedStepName}' cannot be reached by any path through the flow");
            }
        }

        private void VisitStep(int stepIndex, ISet<string> visitedStepNames)
        {
            if (stepIndex >= this.Steps.Count)
            {
                return;
            }

            var flowStep = this.Steps[stepIndex];

            switch (flowStep)
            {
                case ActivityFlowStep _:
                case LabelFlowStep _:
                    if (visitedStepNames.Contains(flowStep.Name))
                    {
                        return;
                    }

                    visitedStepNames.Add(flowStep.Name);

                    var nextStepIndex = stepIndex + 1;
                    VisitStep(nextStepIndex, visitedStepNames);
                    break;

                case DecisionFlowStepBase decisionFlowStep:
                    if (visitedStepNames.Contains(flowStep.Name))
                    {
                        return;
                    }

                    visitedStepNames.Add(flowStep.Name);

                    foreach (var branch in decisionFlowStep.Branches)
                    {
                        var branchStepIndex =
                            string.IsNullOrEmpty(branch.NextStepName)
                                ? stepIndex + 1
                                : GetStepIndex(branch.NextStepName);
                        VisitStep(branchStepIndex, visitedStepNames);
                    }
                    break;

                case GotoFlowStep gotoFlowStep:
                    var gotoStepIndex = GetStepIndex(gotoFlowStep.NextStepName);
                    VisitStep(gotoStepIndex, visitedStepNames);
                    break;

                case EndFlowStep _:
                    return;
            }
        }

        #endregion
    }
}
