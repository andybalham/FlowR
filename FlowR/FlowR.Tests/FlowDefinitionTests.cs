using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlowR.StepLibrary.Activities;
using FlowR.Tests.DiscoveryTarget;
using FlowR.Tests.Domain.FlowTests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using EmptyFlow = FlowR.Tests.DiscoveryTarget.EmptyFlow;

namespace FlowR.Tests
{
    public class FlowDefinitionTests
    {
        #region Facts and theories

        [Fact]
        public void Can_validate_unknown_steps()
        {
            var flowDefinition = new FlowDefinition()
                .Check("Check", FlowValueDecision<int?>.NewDefinition())
                .When(444).Goto("UnknownActivity1")
                .Else().Goto("UnknownActivity2")
                .Goto("UnknownActivity3");

            var validationMessages = flowDefinition.Validate().ToList();

            Assert.Equal(3, validationMessages.Count);

            Assert.Contains(validationMessages, m => m.Contains("UnknownActivity1"));
            Assert.Contains(validationMessages, m => m.Contains("UnknownActivity2"));
            Assert.Contains(validationMessages, m => m.Contains("UnknownActivity3"));
        }

        [Fact]
        public void Can_validate_closed_loops()
        {
            var flowDefinition = new FlowDefinition()
                .Check("Is_int_444", FlowValueDecision<int?>.NewDefinition())
                .When(444).Goto("Activity_3")
                .Else().Continue()

                .Do("Activity_1", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>())
                .Do("Activity_2", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>())

                .Label("Label_1")

                .Check("Is_int_555", FlowValueDecision<int?>.NewDefinition())
                .When(555).Goto("Activity_1")
                .Else().Continue()

                .Check("Is_int_666", FlowValueDecision<int?>.NewDefinition())
                .When(666).Goto("Activity_2")
                .Else().Goto("Activity_1")

                .Goto("Activity_1")

                .Do("Activity_3", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>())

                .Check("Is_int_777", FlowValueDecision<int?>.NewDefinition())
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

                .Check("Is_int_666", FlowValueDecision<int?>.NewDefinition())
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

                .Check("Is_int_666", FlowValueDecision<int?>.NewDefinition())
                .When(666).Goto("Activity_3")
                .Else().Goto("Is_int_555")

                .Do("Orphan_activity_1", new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>())

                .Check("Is_int_555", FlowValueDecision<int?>.NewDefinition())
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

                .Check("Int_decision", FlowValueDecision<int?>.NewDefinition()
                    .BindInput(r => r.SwitchValue, "Input1")
                    .BindInput(r => r.SwitchValue, "Input2"))
                .Else().Continue();

            var validationMessages = flowDefinition.Validate().ToList();

            Assert.NotEmpty(validationMessages);

            Assert.Contains(validationMessages, m => m.Contains("property OutputValue"));
            Assert.Contains(validationMessages, m => m.Contains("property Output"));
            Assert.Contains(validationMessages, m => m.Contains("property SwitchValue"));
        }

        [Fact]
        public async void Can_validate_all_flow_definitions_in_an_assembly()
        {
            var mediator = GetMediator(new TestOverrideProvider());

            var response = await mediator.Send(new FlowValidationRequest());

            Assert.True(
                response.Errors.ContainsKey(typeof(InvalidFlowRequest).FullName), 
                $"response.Errors.ContainsKey({typeof(InvalidFlowRequest).FullName})");

            Assert.True(
                response.Errors.ContainsKey($"{typeof(InvalidFlowRequest).FullName}[OverrideCriteria]"),
                $"response.Errors.ContainsKey({typeof(InvalidFlowRequest).FullName}[OverrideCriteria])");
        }

        #endregion

        #region Private methods and classes

        private class TestOverrideProvider : TestOverrideProviderBase
        {
            public override IEnumerable<FlowDefinition> GetFlowDefinitionOverrides(Type requestType)
            {
                return requestType == typeof(InvalidFlowRequest) 
                    ? new []
                    {
                        new FlowDefinition("OverrideCriteria")
                            .Goto("NonExistentStep")
                    } 
                    : null;
            }

            public override FlowDefinition GetApplicableFlowDefinitionOverride(IList<FlowDefinition> overrides, IFlowStepRequest request)
            {
                return overrides.FirstOrDefault();
            }
        }

        private static IMediator GetMediator(IFlowOverrideProvider overrideProvider = null)
        {
            var serviceCollection =
                new ServiceCollection()
                    .AddMediatR(typeof(IFlowHandler).Assembly);

            if (overrideProvider != null)
            {
                serviceCollection.AddSingleton(overrideProvider);
            }

            typeof(EmptyFlow).Assembly.RegisterFlowTypes(
                (intType, impType) => serviceCollection.AddSingleton(intType, impType));

            if (overrideProvider != null)
            {
                serviceCollection.AddSingleton(overrideProvider);
            }

            var mediator = serviceCollection.BuildServiceProvider().GetService<IMediator>();
            return mediator;
        }

        #endregion
    }
}
