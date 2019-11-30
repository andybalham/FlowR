using FlowR.Tests.Domain.FlowTests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using FlowR.Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace FlowR.Tests
{
    public class FlowTests : TestBase
    {
        public FlowTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async void Empty_flow_runs_as_expected()
        {
            var (mediator, logger) = GetMediator<EmptyFlowRequest>();

            var response = await mediator.Send(new EmptyFlowRequest());

            logger.LogDebug($"response='{response}'");
        }

        [Fact]
        public async void Flow_with_input_and_output_runs_as_expected()
        {
            var (mediator, logger) = GetMediator<InputAndOutputFlowRequest>();

            var request = new InputAndOutputFlowRequest { Value = 616 };

            var response = await mediator.Send(request);

            logger.LogDebug($"response='{response}'");

            Assert.Equal(request.Value, response.Value);
        }

        [Fact]
        public async void Missing_mandatory_flow_response_property_throws_exception()
        {
            var (mediator, _) = GetMediator<MissingMandatoryResponseRequest>();

            var request = new MissingMandatoryResponseRequest();

            var flowException = await Assert.ThrowsAsync<FlowException>(() => mediator.Send(request));

            Assert.Contains(nameof(MissingMandatoryResponseResponse.MandatoryValue), flowException.Message);
            Assert.DoesNotContain(nameof(MissingMandatoryResponseResponse.OptionalValue), flowException.Message);
            Assert.DoesNotContain(nameof(MissingMandatoryResponseResponse.DefaultValue), flowException.Message);
        }

        [Fact]
        public async void Missing_mandatory_flow_request_property_throws_exception()
        {
            var (mediator, _) = GetMediator<MissingMandatoryRequestRequest>();

            var request = new MissingMandatoryRequestRequest();

            var flowException = await Assert.ThrowsAsync<FlowException>(() => mediator.Send(request));

            Assert.Contains(nameof(MissingMandatoryRequestRequest.MandatoryValue), flowException.Message);
            Assert.DoesNotContain(nameof(MissingMandatoryRequestRequest.OptionalValue), flowException.Message);
            Assert.DoesNotContain(nameof(MissingMandatoryRequestRequest.DefaultValue), flowException.Message);
        }

        [Fact]
        public async void Flow_request_and_response_are_logged()
        {
            var loggingOutputBuilder = new StringBuilder();

            new ServiceCollection()
                .AddMediatR(typeof(LoggedInputsAndOutputsFlowRequest).Assembly)
                .AddTransient(typeof(IFlowLogger<>), typeof(CoreFlowLogger<>))
                .AddDebugLogging(this, loggingOutputBuilder)
                .BuildServiceProvider(this, out var mediator, out _);

            var request = new LoggedInputsAndOutputsFlowRequest { PublicValue = 999, PrivateValue = 666 };

            var response = await mediator.Send(request);

            var loggingOutput = loggingOutputBuilder.ToString();

            Assert.False(string.IsNullOrEmpty(loggingOutput));
            Assert.Contains($"Request(PublicValue=999, PrivateValue=***)", loggingOutput);
            Assert.Contains($"Response(PublicValue=999, PrivateValue=***)", loggingOutput);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("CorrelationId", "RequestId")]
        public async void Response_has_flow_ids_populated(string correlationId, string requestId)
        {
            var (mediator, _) = GetMediator<EmptyFlowRequest>();

            var response =
                await mediator.Send(
                    new EmptyFlowRequest { FlowContext = new FlowContext(correlationId, requestId) });

            if (string.IsNullOrEmpty(correlationId))
                Assert.False(string.IsNullOrEmpty(response.CorrelationId));
            else
                Assert.Equal(correlationId, response.CorrelationId);

            if (string.IsNullOrEmpty(requestId))
                Assert.False(string.IsNullOrEmpty(response.RequestId));
            else
                Assert.Equal(requestId, response.RequestId);

            Assert.False(string.IsNullOrEmpty(response.FlowInstanceId));
        }

        [Fact]
        public async void Empty_flow_is_traced()
        {
            var (mediator, _) = GetMediator<EmptyFlowRequest>();

            var response = await mediator.Send(new EmptyFlowRequest());

            Assert.Equal(string.Empty, response.Trace.ToString());
        }

        [Fact]
        public async void Single_activity_flow_is_traced_and_logged()
        {
            var loggingOutputBuilder = new StringBuilder();

            new ServiceCollection()
                .AddMediatR(typeof(SingleActivityFlowRequest).Assembly)
                .AddTransient(typeof(IFlowLogger<>), typeof(CoreFlowLogger<>))
                .AddDebugLogging(this, loggingOutputBuilder)
                .BuildServiceProvider(this, out var mediator, out _);

            var response = await mediator.Send(new SingleActivityFlowRequest());

            Assert.Equal("DoNothing", response.Trace.ToString());

            var loggingOutput = loggingOutputBuilder.ToString();

            Assert.Contains(DoNothingHandler.DebugText, loggingOutput);
        }

        [Fact]
        public async void Activity_output_value_is_returned_from_flow()
        {
            var (mediator, _) = GetMediator<ActivityOutputFlowRequest>();

            var response = await mediator.Send(new ActivityOutputFlowRequest());

            Assert.Equal(SingleOutputActivityResponse.ExpectedOutputValue, response.OutputValue);
        }

        [Fact]
        public async void Activity_input_value_is_returned_from_flow()
        {
            var (mediator, _) = GetMediator<ActivityInputFlowRequest>();

            var request = new ActivityInputFlowRequest { InputValue = Guid.NewGuid() };

            var response = await mediator.Send(request);

            Assert.Equal(request.InputValue, response.OutputValue);
        }

        [Fact]
        public async void Activity_inputs_and_outputs_are_logged()
        {
            var loggingOutputBuilder = new StringBuilder();

            new ServiceCollection()
                .AddMediatR(typeof(ActivityLoggingFlowRequest).Assembly)
                .AddTransient(typeof(IFlowLogger<>), typeof(CoreFlowLogger<>))
                .AddDebugLogging(this, loggingOutputBuilder)
                .BuildServiceProvider(this, out var mediator, out _);

            var request = new ActivityLoggingFlowRequest { PublicInput = Guid.NewGuid(), PrivateInput = Guid.NewGuid() };

            var response = await mediator.Send(request);

            Assert.Equal(request.PublicInput, response.PublicOutput);
            Assert.Equal(request.PrivateInput, response.PrivateOutput);

            var loggingOutput = loggingOutputBuilder.ToString();

            Assert.False(string.IsNullOrEmpty(loggingOutput));

            Assert.Contains($"Request(PublicInput={request.PublicInput}, PrivateInput=***)", loggingOutput);
            Assert.Contains($"Response(PublicOutput={response.PublicOutput}, PrivateOutput=***)", loggingOutput);
        }

        [Fact]
        public async void Activity_design_time_value_is_returned_from_flow()
        {
            var (mediator, _) = GetMediator<ActivityDesignTimeFlowRequest>();

            var request = new ActivityDesignTimeFlowRequest();

            var response = await mediator.Send(request);

            Assert.Equal(ActivityDesignTimeFlow.DesignTimeValue, response.OutputValue);
        }

        [Fact]
        public async void Activity_can_have_static_input_and_output_flow_value_bindings()
        {
            var (mediator, _) = GetMediator<ActivityStaticValueBindingsFlowRequest>();

            var request = new ActivityStaticValueBindingsFlowRequest() { FlowInput = Guid.NewGuid().ToString() };

            var response = await mediator.Send(request);

            Assert.Equal(request.FlowInput, response.FlowOutput);
        }

        [Fact]
        public async void Activity_can_have_overridden_setters()
        {
            var (mediator, _) = GetMediator<ActivityOverriddenSettersFlowRequest>();

            var request = new ActivityOverriddenSettersFlowRequest() { FlowInputValue = Guid.NewGuid().ToString() };

            var response = await mediator.Send(request);

            Assert.Equal(request.FlowInputValue, response.FlowOutputValue);
            Assert.Equal(ActivityOverriddenSettersFlowRequest.NonExistentSetterValue, response.NonExistentSetterOutputValue);
            Assert.Null(response.NullSetterOutputValue);
        }

        [Theory]
        [InlineData(1, null, "X", "Int_value ? 1 -> Set_output_to_X")]
        [InlineData(2, null, "Y", "Int_value ? 2|3 -> Set_output_to_Y")]
        [InlineData(3, null, "Y", "Int_value ? 2|3 -> Set_output_to_Y")]
        [InlineData(4, "A", "ZA", "Int_value ? ELSE -> String_value ? A -> Set_output_to_ZA")]
        [InlineData(4, "B", "ZZ", "Int_value ? ELSE -> String_value ? ELSE -> Set_output_to_ZZ")]
        public async void Match_decision_switches_as_expected(int intValue, string stringValue,
            string expectedOutputValue, string expectedFlowTrace)
        {
            var (mediator, _) = GetMediator<MatchDecisionFlowRequest>();

            var request = new MatchDecisionFlowRequest { IntValue = intValue, StringValue = stringValue };

            var response = await mediator.Send(request);

            Assert.Equal(expectedOutputValue, response.BranchValue);

            Assert.Equal(expectedFlowTrace, response.Trace.ToString());
        }

        [Fact]
        public async void Multiple_match_decision_throws_exception()
        {
            var (mediator, _) = GetMediator<MultipleMatchDecisionFlowRequest>();

            var request = new MultipleMatchDecisionFlowRequest { IntValue = 1 };

            var flowException = await Assert.ThrowsAsync<FlowException>(async () => await mediator.Send(request));

            Assert.Matches("Found multiple matching branches", flowException.Message);
        }

        [Theory]
        [InlineData("A", "X", "Set_output_to_X -> Switch_value_1 ? A")]
        [InlineData("B", "Y", "Set_output_to_X -> Switch_value_1 ? B -> Set_output_to_Y -> Switch_value_2 ? ELSE")]
        [InlineData("C", "Z", "Set_output_to_X -> Switch_value_1 ? ELSE -> Set_output_to_Y -> Switch_value_2 ? C -> Set_output_to_Z")]
        public async void Match_decision_with_end_switches_as_expected(string stringValue,
            string expectedOutputValue, string expectedFlowTrace)
        {
            var (mediator, _) = GetMediator<DecisionWithEndFlowRequest>();

            var request = new DecisionWithEndFlowRequest { StringValue = stringValue };

            var response = await mediator.Send(request);

            Assert.Equal(expectedOutputValue, response.BranchValue);

            Assert.Equal(expectedFlowTrace, response.Trace.ToString());
        }

        [Fact]
        public async void Labels_and_gotos_are_supported()
        {
            var (mediator, _) = GetMediator<LabelsAndGotosFlowRequest>();

            var response = await mediator.Send(new LabelsAndGotosFlowRequest());

            Assert.Equal("#Label_one -> #Label_three", response.Trace.ToString());
        }

        [Fact]
        public async void Activity_binding_name_attributes_are_implemented()
        {
            var (mediator, _) = GetMediator<ActivityBindingAttributesFlowRequest>();

            var request = new ActivityBindingAttributesFlowRequest { FlowInput1 = "FlowInput1", FlowInput2 = "FlowInput2" };

            var response = await mediator.Send(request);

            Assert.Equal(request.FlowInput1, response.FlowOutput1);
            Assert.Equal(request.FlowInput2, response.FlowOutput2);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async void Decision_binding_name_attributes_are_implemented(bool flowInput1, bool flowInput2)
        {
            var (mediator, _) = GetMediator<DecisionBindingAttributesFlowRequest>();

            var request = new DecisionBindingAttributesFlowRequest { FlowInput1 = flowInput1, FlowInput2 = flowInput2 };

            var response = await mediator.Send(request);

            var expectedFlowOutput = request.FlowInput1 && request.FlowInput2;

            Assert.Equal(expectedFlowOutput, response.FlowOutput);
        }

        [Fact]
        public async void Can_bind_properties_to_sub_properties()
        {
            var (mediator, _) = GetMediator<SubPropertyBindingsFlowRequest>();

            var request = new SubPropertyBindingsFlowRequest { StringValue1 = "Bananas!", StringValue2 = "Gelato!" };

            var response = await mediator.Send(request);

            Assert.Equal(request.StringValue1.Length.ToString(), response.StringValue1);
            Assert.Equal(request.StringValue2.Length.ToString(), response.StringValue2);
        }

        [Fact]
        public async void Can_have_enableable_steps()
        {
            var (mediator, _) = GetMediator<EnableableStepsFlowRequest>();

            var request = new EnableableStepsFlowRequest();

            var response = await mediator.Send(request);

            Assert.DoesNotMatch("Activity_Default", response.Trace.ToString());
            Assert.Matches("Activity_Enabled", response.Trace.ToString());
        }

        [Fact]
        public async void Can_have_disableable_steps()
        {
            var (mediator, _) = GetMediator<DisableableStepsFlowRequest>();

            var request = new DisableableStepsFlowRequest();

            var response = await mediator.Send(request);

            Assert.Matches("Activity_Default", response.Trace.ToString());
            Assert.DoesNotMatch("Activity_Disabled", response.Trace.ToString());
        }

        [Fact]
        public async void Can_validate_mandatory_request_input_properties()
        {
            var (mediator, _) = GetMediator<MissingMandatoryActivityInputRequest>();

            var request = new MissingMandatoryActivityInputRequest();

            var flowException = await Assert.ThrowsAsync<FlowException>(async () => await mediator.Send(request));

            Assert.Contains(nameof(MandatoryInputActivityRequest.MandatorySetInput), flowException.Message);
            Assert.Contains(nameof(MandatoryInputActivityRequest.MandatoryBoundInput), flowException.Message);
            Assert.DoesNotContain(nameof(MandatoryInputActivityRequest.OptionalSetInput), flowException.Message);
            Assert.DoesNotContain(nameof(MandatoryInputActivityRequest.OptionalBoundInput), flowException.Message);
            Assert.DoesNotContain(nameof(MandatoryInputActivityRequest.DefaultBoundInput), flowException.Message);
        }

        [Fact]
        public async void Can_validate_mandatory_request_output_properties()
        {
            var (mediator, _) = GetMediator<MissingMandatoryActivityOutputRequest>();

            var request = new MissingMandatoryActivityOutputRequest();

            var flowException = await Assert.ThrowsAsync<FlowException>(async () => await mediator.Send(request));

            Assert.Contains(nameof(MandatoryOutputActivityResponse.MandatoryOutput), flowException.Message);
            Assert.DoesNotContain(nameof(MandatoryOutputActivityResponse.OptionalOutput), flowException.Message);
        }

        [Fact]
        public async void Can_bind_default_flow_value_dictionary()
        {
            var (mediator, _) = GetMediator<FlowValueDictionaryDefaultRequest>();

            var request = new FlowValueDictionaryDefaultRequest
            {
                StringInput1 = "X",
                StringInput2 = "Y",
                IntInput1 = 1,
                IntInput2 = 2
            };

            var response = await mediator.Send(request);

            Assert.Equal(request.StringInput1, response.StringOutput1);
            Assert.Equal(request.StringInput2, response.StringOutput2);

            Assert.Equal(0, response.IntOutput1);
            Assert.Equal(0, response.IntOutput2);
        }

        [Fact]
        public async void Can_bind_multiple_values_by_list()
        {
            var (mediator, _) = GetMediator<TestDictionaryListBindingsRequest>();

            var request = new TestDictionaryListBindingsRequest { FlowInput1 = "X", FlowInput2 = "Y", FlowInput3 = "Z" };

            var response = await mediator.Send(request);

            Assert.Equal(request.FlowInput1, response.FlowOutput1);
            Assert.NotEqual(request.FlowInput2, response.FlowOutput2);
            Assert.NotEqual(request.FlowInput3, response.FlowOutput3);
        }

        [Fact]
        public async void Can_bind_multiple_values_by_list_map()
        {
            var (mediator, _) = GetMediator<TestDictionaryListMapBindingsRequest>();

            var request = new TestDictionaryListMapBindingsRequest { RequestValue1 = "X", RequestValue2 = "Y", RequestValue3 = "Z" };

            var response = await mediator.Send(request);

            Assert.Equal(request.RequestValue1, response.ResponseValue1);
            Assert.NotEqual(request.RequestValue2, response.ResponseValue2);
            Assert.NotEqual(request.RequestValue3, response.ResponseValue3);
        }

        [Fact]
        public async void Can_bind_multiple_values_to_dictionary_by_regex()
        {
            var (mediator, _) = GetMediator<TestDictionaryRegexBindingsRequest>();

            var request = new TestDictionaryRegexBindingsRequest { FlowInput1 = "X", FlowInput2 = "Y", NonMatchInput = "Z" };

            var response = await mediator.Send(request);

            Assert.Equal(request.FlowInput1, response.FlowOutput1);
            Assert.Equal(request.FlowInput2, response.FlowOutput2);
            Assert.NotEqual(request.NonMatchInput, response.NonMatchOutput);
        }

        [Fact]
        public async void Can_bind_multiple_values_to_dictionary_by_regex_map()
        {
            var (mediator, _) = GetMediator<TestDictionaryRegexMapBindingsRequest>();

            var request = new TestDictionaryRegexMapBindingsRequest { RequestValue1 = "X", RequestValue2 = "Y", NonMatchInput = "Z" };

            var response = await mediator.Send(request);

            Assert.Equal(request.RequestValue1, response.ResponseValue1);
            Assert.Equal(request.RequestValue2, response.ResponseValue2);
            Assert.NotEqual(request.NonMatchInput, response.NonMatchOutput);
        }

        [Fact]
        public async void Can_bind_multiple_sub_property_values_to_dictionary()
        {
            var (mediator, _) = GetMediator<TestDictionarySubPropertyBindingsRequest>();

            var request =
                new TestDictionarySubPropertyBindingsRequest
                {
                    NamedInput1 = 0,
                    NamedInput2 = 1,
                    RegexInput1 = 2,
                    RegexInput2 = 3,
                };

            var response = await mediator.Send(request);

            Assert.Equal(0 + 1 + 1, response.NamedOutput1);
            Assert.Equal(1 + 1 + 1, response.NamedOutput2);
            Assert.Equal(2 + 2 + 2, response.RegexOutput1);
            Assert.Equal(3 + 2 + 2, response.RegexOutput2);
        }

        [Fact]
        public async void Can_bind_dictionary_inputs_for_decisions()
        {
            var (mediator, _) = GetMediator<TestDecisionDictionaryBindingRequest>();

            var request = new TestDecisionDictionaryBindingRequest
            {
                String1 = "StringValue1",
                String2 = "StringValue2",
                String3 = "StringValue3",
            };

            await mediator.Send(request);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("Any old value", true)]
        public async void Can_throw_exception_if_no_matching_branch(string input, bool isExceptionExpected)
        {
            // Arrange

            var (mediator, _) = GetMediator<TestElseThrowRequest>();

            var request = new TestElseThrowRequest { Input = input };

            try
            {
                // Act

                await mediator.Send(request);

                // Assert

                Assert.False(isExceptionExpected, "isExceptionExpected");
            }
            catch (FlowUnhandledElseException e)
            {
                // Assert

                Assert.True(isExceptionExpected, "isExceptionExpected");
                Assert.Matches("UnhandledDecisionName", e.Message);
            }
        }
    }

    public class TestElseThrowRequest : FlowActivityRequest<TestElseThrowResponse>
    {
        public string Input { get; set; }
    }

    public class TestElseThrowResponse : FlowResponse
    {
    }

    public class TestElseThrowHandler : FlowHandler<TestElseThrowRequest, TestElseThrowResponse>
    {
        public TestElseThrowHandler(IMediator mediator) : base(mediator)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition<TestElseThrowRequest, TestElseThrowResponse> flowDefinition)
        {
            flowDefinition
                .Check("UnhandledDecisionName", NullableFlowValueDecision<string>.NewDefinition()
                    .BindInput(rq => rq.SwitchValue, nameof(TestElseThrowRequest.Input)))
                .When((string)null).End()
                .Else().Unhandled();
        }
    }
}
