using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlowR.StepLibrary.Activities;
using FlowR.StepLibrary.Decisions;
using FlowR.Tests.Domain.FlowTests;
using Xunit;

namespace FlowR.Tests
{
    public class FlowDefinitionTests
    {
        [Fact]
        public void Can_validate_closed_loops()
        {
            var flowDefinition = new FlowDefinition()
                .Check("Is_int_444", new FlowDecisionDefinition<IntFlowValueDecision, int?>())
                .When(444).Goto("Activity_3")
                .Else().Continue()

                .Do("Activity_1", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>())
                .Do("Activity_2", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>())

                .Label("Label_1")

                .Check("Is_int_555", new FlowDecisionDefinition<IntFlowValueDecision, int?>())
                .When(555).Goto("Activity_1")
                .Else().Continue()

                .Check("Is_int_666", new FlowDecisionDefinition<IntFlowValueDecision, int?>())
                .When(666).Goto("Activity_2")
                .Else().Goto("Activity_1")

                .Goto("Activity_1")

                .Do("Activity_3", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>())

                .Check("Is_int_777", new FlowDecisionDefinition<IntFlowValueDecision, int?>())
                .When(666).Goto("Activity_3")
                .Else().Continue();

            var validationMessages = flowDefinition.Validate().ToList();

            Assert.Equal(5, validationMessages.Count);

            Assert.Contains(validationMessages, m => m.Contains("Activity_1"));
            Assert.Contains(validationMessages, m => m.Contains("Activity_2"));
            Assert.Contains(validationMessages, m => m.Contains("Label_1"));
            Assert.Contains(validationMessages, m => m.Contains("Is_int_555"));
            Assert.Contains(validationMessages, m => m.Contains("Is_int_666"));

            Assert.DoesNotContain(validationMessages, m => m.Contains("Is_int_444"));
            Assert.DoesNotContain(validationMessages, m => m.Contains("Activity_3"));
            Assert.DoesNotContain(validationMessages, m => m.Contains("Is_int_777"));
        }

        [Fact]
        public void Can_accept_open_loops()
        {
            var flowDefinition = new FlowDefinition()
                .Do("Activity_1", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>())
                .Do("Activity_2", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>())

                .Check("Is_int_666", new FlowDecisionDefinition<IntFlowValueDecision, int?>())
                .When(666).Goto("Activity_3")
                .Else().Continue()

                .Goto("Activity_1")

                .Do("Activity_3", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>());

            // TODO: Only allow loops if a flag is explicitly set
            var validationMessages = flowDefinition.Validate().ToList();

            Assert.Empty(validationMessages);
        }

        [Fact]
        public void Can_validate_orphaned_steps()
        {
            var flowDefinition = new FlowDefinition()
                .Do("Activity_1", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>())

                .Check("Is_int_666", new FlowDecisionDefinition<IntFlowValueDecision, int?>())
                .When(666).Goto("Activity_3")
                .Else().Goto("Is_int_555")

                .Do("Orphan_activity_1", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>())

                .Check("Is_int_555", new FlowDecisionDefinition<IntFlowValueDecision, int?>())
                .When(555).Goto("End_activity_label")
                .Else().Continue()

                .Do("Activity_2", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>())
                .Goto("Activity_3")

                .Do("Orphan_activity_2", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>())

                .Do("Activity_3", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>())
                .End()

                .Do("Orphan_activity_3", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>())

                .Label("End_activity_label")
                .Do("Activity_4", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>());

            var validationMessages = flowDefinition.Validate().ToList();

            Assert.NotEmpty(validationMessages);

            Assert.Contains(validationMessages, m => m.Contains("Orphan_activity_1"));
            Assert.Contains(validationMessages, m => m.Contains("Orphan_activity_2"));
            Assert.Contains(validationMessages, m => m.Contains("Orphan_activity_3"));

            Assert.DoesNotContain(validationMessages, m => m.Contains("Activity_1"));
            Assert.DoesNotContain(validationMessages, m => m.Contains("Activity_2"));
            Assert.DoesNotContain(validationMessages, m => m.Contains("Activity_3"));
            Assert.DoesNotContain(validationMessages, m => m.Contains("Activity_4"));
            Assert.DoesNotContain(validationMessages, m => m.Contains("Is_int_555"));
            Assert.DoesNotContain(validationMessages, m => m.Contains("Is_int_666"));
            Assert.DoesNotContain(validationMessages, m => m.Contains("End_activity_label"));
        }

        [Fact]
        public void Can_validate_duplicate_sets_and_bindings_in_definitions()
        {
            // TODO: Allow multiple output bindings as we could be setting multiple values from properties
            var flowDefinition = new FlowDefinition()
                .Do("Set_value", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                    .SetValue(r => r.OutputValue, "Value1")
                    .SetValue(r => r.OutputValue, "Value2")
                    .BindOutput(r => r.Output, "Output1")
                    .BindOutput(r => r.Output, "Output2"))

                .Check("Int_decision", new FlowDecisionDefinition<IntFlowValueDecision, int?>()
                    .BindInput(r => r.SwitchValue, "Input1")
                    .BindInput(r => r.SwitchValue, "Input2"))
                .Else().Continue();

            var validationMessages = flowDefinition.Validate().ToList();

            Assert.NotEmpty(validationMessages);

            Assert.Contains(validationMessages, m => m.Contains("property OutputValue"));
            Assert.Contains(validationMessages, m => m.Contains("property Output"));
            Assert.Contains(validationMessages, m => m.Contains("property SwitchValue"));
        }

        [Fact(Skip = "Implement this as part of discovery")]
        public void Can_validate_all_flow_definitions_in_an_assembly()
        {
        }
    }
}
