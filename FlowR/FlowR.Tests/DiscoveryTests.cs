using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using FlowR.Discovery;
using FlowR.Tests.DiscoveryTarget;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace FlowR.Tests
{
    public class DiscoveryTests : TestBase
    {
        #region Constructors

        public DiscoveryTests(ITestOutputHelper output) : base(output)
        {
        }

        #endregion

        #region Facts and theories

        [Fact]
        public async void Can_discover_empty_flow()
        {
            var mediator = GetMediator();

            var flowDiscoveryResponse = await mediator.Send(new FlowDiscoveryRequest());

            Assert.NotNull(flowDiscoveryResponse.Flows);
            Assert.Contains(flowDiscoveryResponse.Flows, flow => flow.Request.RequestType == typeof(EmptyFlowRequest));
        }

        [Fact]
        public async void Empty_flow_has_expected_diagram()
        {
            var mediator = GetMediator();

            var flowDiscoveryResponse = await mediator.Send(new FlowDiscoveryRequest());
            Assert.NotNull(flowDiscoveryResponse.Flows);

            var flow = flowDiscoveryResponse.Flows.First(f => f.Request.RequestType == typeof(EmptyFlowRequest));
            Assert.NotNull(flow);

            var expectedLines = new[]
            {
                "Start -> End_1;"
            };

            AssertDiagramFlow(flow, expectedLines);
        }

        [Fact]
        public async void Sequential_flow_has_expected_diagram()
        {
            var mediator = GetMediator();

            var flowDiscoveryResponse = await mediator.Send(new FlowDiscoveryRequest());
            Assert.NotNull(flowDiscoveryResponse.Flows);

            var flow = flowDiscoveryResponse.Flows.First(f => f.Request.RequestType == typeof(SequentialFlowRequest));
            Assert.NotNull(flow);

            var expectedLines = new[]
            {
                "Start -> Label_1;",

                "Label_1 -> Activity_1;",
                "Activity_1 -> Label_2;",

                "Label_2 -> Activity_2;",
                "Activity_2 -> Label_3;",

                "Label_3 -> Activity_3;",
                "Activity_3 -> End_1;",
            };

            AssertDiagramFlow(flow, expectedLines);
        }

        [Fact]
        public async void Decision_flow_has_expected_diagram()
        {
            var mediator = GetMediator();

            var flowDiscoveryResponse = await mediator.Send(new FlowDiscoveryRequest());
            Assert.NotNull(flowDiscoveryResponse.Flows);

            var flow = flowDiscoveryResponse.Flows.First(f => f.Request.RequestType == typeof(DecisionFlowRequest));
            Assert.NotNull(flow);

            var expectedLines = new[]
            {
                "Start -> Label_1;",

                "Label_1 -> Decision_1;",

                "Decision_1 -> End_1 [label=\"X\"];",
                "Decision_1 -> Decision_2;",

                "Decision_2 -> Label_1 [label=\"Y or Z\"];",
                "Decision_2 -> End_2;",
            };

            AssertDiagramFlow(flow, expectedLines);
        }

        [Fact]
        public async void Flow_diagram_nodes_contain_setters_and_bindings()
        {
            var mediator = GetMediator();

            var flowDiscoveryResponse = await mediator.Send(new FlowDiscoveryRequest());
            Assert.NotNull(flowDiscoveryResponse.Flows);

            var flow = flowDiscoveryResponse.Flows.First(f => f.Request.RequestType == typeof(ConfiguredFlowRequest));
            Assert.NotNull(flow);

            _output.WriteLine(JsonConvert.SerializeObject(flow));

            var activityNode = flow.Nodes.First(n => n.Name == "Activity_1");
            Assert.NotNull(activityNode);

            Assert.Equal(activityNode.Name, activityNode.Text);

            Assert.NotNull(activityNode.InputSetters);
            Assert.True(activityNode.InputSetters.ContainsKey(nameof(ConfigurableActivityRequest.SetValue)));
            Assert.Equal("SetValue", activityNode.InputSetters[nameof(ConfigurableActivityRequest.SetValue)]);

            Assert.NotNull(activityNode.InputBindings);
            Assert.True(activityNode.InputBindings.ContainsKey(nameof(ConfigurableActivityRequest.BoundValue)));
            Assert.Equal("FlowValue", activityNode.InputBindings[nameof(ConfigurableActivityRequest.BoundValue)]);

            Assert.NotNull(activityNode.OutputBindings);
            Assert.True(activityNode.OutputBindings.ContainsKey(nameof(ConfigurableActivityResponse.OutputValue)));
            Assert.Equal("FlowValue: OutputValue", activityNode.OutputBindings[nameof(ConfigurableActivityResponse.OutputValue)]);

            var decisionNode = flow.Nodes.First(n => n.Name == "Decision_1");
            Assert.NotNull(decisionNode);

            Assert.Equal($"{decisionNode.Name}?", decisionNode.Text);

            Assert.NotNull(decisionNode.InputSetters);
            Assert.True(decisionNode.InputSetters.ContainsKey(nameof(ConfigurableActivityRequest.SetValue)));
            Assert.Equal("SetValue", decisionNode.InputSetters[nameof(ConfigurableActivityRequest.SetValue)]);

            Assert.NotNull(decisionNode.InputBindings);
            Assert.True(decisionNode.InputBindings.ContainsKey(nameof(ConfigurableActivityRequest.BoundValue)));
            Assert.Equal("FlowValue", decisionNode.InputBindings[nameof(ConfigurableActivityRequest.BoundValue)]);
        }

        [Fact]
        public async void Flow_diagram_nodes_contain_default_or_request_or_overridden_text_as_appropriate()
        {
            var mediator = GetMediator();

            var flowDiscoveryResponse = await mediator.Send(new FlowDiscoveryRequest());
            Assert.NotNull(flowDiscoveryResponse.Flows);

            var flow = flowDiscoveryResponse.Flows.First(f => f.Request.RequestType == typeof(TextTestFlowRequest));
            Assert.NotNull(flow);

            _output.WriteLine(JsonConvert.SerializeObject(flow));

            var activity1Node = flow.Nodes.First(n => n.Name == "Activity_1");
            Assert.NotNull(activity1Node);
            Assert.Equal(activity1Node.Name, activity1Node.Text);

            var activity2Node = flow.Nodes.First(n => n.Name == "Activity_2");
            Assert.NotNull(activity2Node);
            Assert.Equal("SetValue=SetValue", activity2Node.Text);

            var activity3Node = flow.Nodes.First(n => n.Name == "Activity_3");
            Assert.NotNull(activity3Node);
            Assert.Equal("Custom text", activity3Node.Text);

            var decision1Node = flow.Nodes.First(n => n.Name == "Decision_1");
            Assert.NotNull(decision1Node);
            Assert.Equal($"{decision1Node.Name}?", decision1Node.Text);

            var decision2Node = flow.Nodes.First(n => n.Name == "Decision_2");
            Assert.NotNull(decision2Node);
            Assert.Equal("SetValue=SetValue", decision2Node.Text);

            var decision3Node = flow.Nodes.First(n => n.Name == "Decision_3");
            Assert.NotNull(decision3Node);
            Assert.Equal("Custom text", decision3Node.Text);

            var label1Node = flow.Nodes.First(n => n.Name == "Label_1");
            Assert.NotNull(label1Node);
            Assert.Equal(label1Node.Name, label1Node.Text);

            var label2Node = flow.Nodes.First(n => n.Name == "Label_2");
            Assert.NotNull(label2Node);
            Assert.Equal("Custom text", label2Node.Text);
        }

        [Fact]
        public async void Flow_diagram_nodes_contain_override_details()
        {
            var mediator = GetMediator();

            var flowDiscoveryResponse = await mediator.Send(new FlowDiscoveryRequest());
            Assert.NotNull(flowDiscoveryResponse.Flows);

            var flow = flowDiscoveryResponse.Flows.First(f => f.Request.RequestType == typeof(TextTestFlowRequest));
            Assert.NotNull(flow);

            _output.WriteLine(JsonConvert.SerializeObject(flow));

            var activity1Node = flow.Nodes.First(n => n.Name == "Activity_1");
            Assert.NotNull(activity1Node);
            Assert.Equal("Activity_1-OverrideKey", activity1Node.OverrideKey);
            Assert.Equal("Activity_1 override description", activity1Node.OverrideDescription);

            var decision1Node = flow.Nodes.First(n => n.Name == "Decision_1");
            Assert.NotNull(decision1Node);
            Assert.Equal("Decision_1-OverrideKey", decision1Node.OverrideKey);
            Assert.Equal("Decision_1 override description", decision1Node.OverrideDescription);
        }

        [Fact]
        public async void Flow_diagram_contains_request_and_response_summaries()
        {
            var mediator = GetMediator();

            var flowDiscoveryResponse = await mediator.Send(new FlowDiscoveryRequest());
            Assert.NotNull(flowDiscoveryResponse.Flows);

            var flow = flowDiscoveryResponse.Flows.First(f => f.Request.RequestType == typeof(TextTestFlowRequest));
            Assert.NotNull(flow);

            _output.WriteLine(JsonConvert.SerializeObject(flow));

            Assert.NotNull(flow.Request);
            Assert.Equal(typeof(TextTestFlowRequest), flow.Request.RequestType);
            Assert.Equal("Text test request description", flow.Request.Description);
            Assert.NotNull(flow.Request.Properties);

            var inputPropertyInfo = flow.Request.Properties.First(p => p.Name == nameof(TextTestFlowRequest.InputValue));
            Assert.NotNull(inputPropertyInfo);
            Assert.Equal("Text test input value description", inputPropertyInfo.Description);
            Assert.Equal(typeof(string), inputPropertyInfo.PropertyType);

            Assert.NotNull(flow.Response);
            Assert.Equal(typeof(TextTestFlowResponse), flow.Response.ResponseType);
            Assert.Equal("Text test response description", flow.Response.Description);
            Assert.NotNull(flow.Response.Properties);

            var outputPropertyInfo = flow.Response.Properties.First(p => p.Name == nameof(TextTestFlowResponse.OutputValue));
            Assert.NotNull(outputPropertyInfo);
            Assert.Equal("Text test output value description", outputPropertyInfo.Description);
            Assert.Equal(typeof(string), outputPropertyInfo.PropertyType);
        }

        [Fact]
        public async void Can_discover_flow_overrides()
        {
            var mediator = GetMediator(new EmptyFlowRequestOverrideProvider());

            var flowDiscoveryResponse = await mediator.Send(new FlowDiscoveryRequest());

            Assert.NotNull(flowDiscoveryResponse.Flows);

            var flowDiagrams =
                flowDiscoveryResponse.Flows.Where(flow =>
                    flow.Request.RequestType == typeof(EmptyFlowRequest)).ToList();

            Assert.Equal(2, flowDiagrams.Count);
            Assert.Equal(1, flowDiagrams.Count(f => string.IsNullOrEmpty(f.Criteria)));
            Assert.Equal(1, flowDiagrams.Count(f => string.Equals(f.Criteria, EmptyFlowRequestOverrideProvider.FlowCriteria)));
        }

        private class EmptyFlowRequestOverrideProvider : TestOverrideProviderBase
        {
            public const string FlowCriteria = "FlowCriteria";

            public override IEnumerable<FlowDefinition> GetFlowDefinitionOverrides(Type requestType)
            {
                return requestType == typeof(EmptyFlowRequest) ? new[] { new FlowDefinition(FlowCriteria) } : null;
            }
        }

        [Fact]
        public async void Can_discover_flow_with_default_override()
        {
            var mediator = GetMediator(new EmptyFlowRequestDefaultOverrideProvider());

            var flowDiscoveryResponse = await mediator.Send(new FlowDiscoveryRequest());

            Assert.NotNull(flowDiscoveryResponse.Flows);

            var flowDiagrams =
                flowDiscoveryResponse.Flows.Where(flow =>
                    flow.Request.RequestType == typeof(EmptyFlowRequest)).ToList();

            Assert.Equal(2, flowDiagrams.Count);
            Assert.Equal(2, flowDiagrams.Count(f => string.IsNullOrEmpty(f.Criteria)));
            Assert.Equal(1, flowDiagrams.Count(f => f.IsOverride));
        }

        private class EmptyFlowRequestDefaultOverrideProvider : TestOverrideProviderBase
        {
            public override IEnumerable<FlowDefinition> GetFlowDefinitionOverrides(Type requestType) => 
                requestType == typeof(EmptyFlowRequest) ? new[] { new FlowDefinition() } : null;
        }

        [Fact]
        public async void Can_discover_flow_with_request_overrides()
        {
            var mediator = GetMediator(new ActivityRequestOverrideProvider());

            var flowDiscoveryResponse = await mediator.Send(new FlowDiscoveryRequest());

            Assert.NotNull(flowDiscoveryResponse.Flows);

            var flowDiagram =
                flowDiscoveryResponse.Flows.FirstOrDefault(flow =>
                    flow.Request.RequestType == typeof(OverriddenActivityFlowRequest));

            Assert.NotNull(flowDiagram);

            var overriddenNode = 
                flowDiagram.Nodes.FirstOrDefault(n => n.OverrideKey == OverriddenActivityFlowRequest.ActivityOverrideKey);

            Assert.NotNull(overriddenNode);

            Assert.Equal(1, overriddenNode.InputOverrides.Count);

            Assert.True(
                overriddenNode.InputOverrides.TryGetValue(
                    nameof(OverridableActivityRequest.OverridableInputValue), out var propertyInputOverrides));

            Assert.Equal(2, propertyInputOverrides.Count);

            var propertyInputOverride1 = 
                propertyInputOverrides.FirstOrDefault(o => o.Criteria == ActivityRequestOverrideProvider.OverrideCriteria1);
            Assert.NotNull(propertyInputOverride1);
            Assert.Equal(ActivityRequestOverrideProvider.OverrideValue1, propertyInputOverride1.Value);

            var propertyInputOverride2 =
                propertyInputOverrides.FirstOrDefault(o => o.Criteria == ActivityRequestOverrideProvider.OverrideCriteria2);
            Assert.NotNull(propertyInputOverride2);
            Assert.Equal(ActivityRequestOverrideProvider.OverrideValue2, propertyInputOverride2.Value);
        }

        [Fact]
        public async void Flow_with_request_overrides_has_highlighting()
        {
            var mediator = GetMediator(new ActivityRequestOverrideProvider());

            var flowDiscoveryResponse = await mediator.Send(new FlowDiscoveryRequest());

            Assert.NotNull(flowDiscoveryResponse.Flows);

            var flowDiagram =
                flowDiscoveryResponse.Flows.FirstOrDefault(flow =>
                    flow.Request.RequestType == typeof(OverriddenActivityFlowRequest));

            Assert.NotNull(flowDiagram);

            var dotNotation = flowDiagram.GetDotNotation();

            Assert.Matches($"Activity.*color={FlowDiagram.RequestOverrideColor}", dotNotation);
        }

        private class ActivityRequestOverrideProvider : TestOverrideProviderBase
        {
            public const string OverrideValue1 = "OverrideValue1";
            public const string OverrideCriteria1 = "OverrideCriteria1";
            public const string OverrideValue2 = "OverrideValue2";
            public const string OverrideCriteria2 = "OverrideCriteria2";

            public override IEnumerable<FlowRequestOverride> GetRequestOverrides(string overrideKey)
            {
                switch (overrideKey)
                {
                    case OverriddenActivityFlowRequest.ActivityOverrideKey:
                        return new[]
                        {
                            new FlowRequestOverride
                            {
                                Name = nameof(OverridableActivityRequest.OverridableInputValue),
                                Value = OverrideValue1,
                                Criteria = OverrideCriteria1
                            },
                            new FlowRequestOverride
                            {
                                Name = nameof(OverridableActivityRequest.OverridableInputValue),
                                Value = OverrideValue2,
                                Criteria = OverrideCriteria2
                            },
                        };
                }

                return null;
            }
        }

        private class TestRequest : IFlowStepRequest
        {
            public FlowContext FlowContext { get; set; }

            public string GetText() => null;

            public int InputInt { get; set; }

            public FlowValueDictionary<string> InputStrings { get; set; }
        }

        public class TestResponse
        {
            public int OutputInt { get; set; }

            public FlowValueDictionary<string> OutputStrings { get; set; }
        }

        [Theory]
        [InlineData(false, "FlowInt")]
        [InlineData(true, "Func(FlowInt)")]
        public void Input_binding_summary_for_single_value_as_expected(bool isMappedValue, string expectedSummary)
        {
            var request = new TestRequest();

            var requestType = request.GetType().GetFlowObjectType();
            var requestProperty = requestType[nameof(TestRequest.InputInt)];

            var inputBinding = new FlowValueInputBinding(requestProperty)
            {
                FlowValueSelector = new FlowValueSingleSelector("FlowInt"),
                MapValue = isMappedValue ? v => v : (Func<object, object>)null,
            };

            var summary = inputBinding.GetSummary(request);

            Assert.Equal(expectedSummary, summary);
        }

        [Theory]
        [InlineData(false, "{ FlowValue1, FlowValue2 }")]
        [InlineData(true, "{ Func(FlowValue1), Func(FlowValue2) }")]
        public void Input_binding_summary_for_dictionary_list_values_as_expected(bool isMappedValue, string expectedSummary)
        {
            var request = new TestRequest();

            var requestType = request.GetType().GetFlowObjectType();
            var requestProperty = requestType[nameof(TestRequest.InputStrings)];

            var inputBinding = new FlowValueInputBinding(requestProperty)
            {
                FlowValueSelector = new FlowValueListSelector("FlowValue1", "FlowValue2"),
                MapValue = isMappedValue ? v => v : (Func<object, object>)null,
            };

            var summary = inputBinding.GetSummary(request);

            Assert.Equal(expectedSummary, summary);
        }

        [Theory]
        [InlineData(false, "{ DictionaryValue1: FlowValue1, DictionaryValue2: FlowValue2 }")]
        [InlineData(true, "{ DictionaryValue1: Func(FlowValue1), DictionaryValue2: Func(FlowValue2) }")]
        public void Input_binding_summary_for_dictionary_value_name_map_as_expected(bool isMappedValue, string expectedSummary)
        {
            var request = new TestRequest();

            var requestType = request.GetType().GetFlowObjectType();
            var requestProperty = requestType[nameof(TestRequest.InputStrings)];

            var inputBinding = new FlowValueInputBinding(requestProperty)
            {
                FlowValueSelector = new FlowValueListSelector(new Dictionary<string, string>
                {
                    { "FlowValue1", "DictionaryValue1" },
                    { "FlowValue2", "DictionaryValue2" },
                }),
                MapValue = isMappedValue ? v => v : (Func<object, object>)null,
            };

            var summary = inputBinding.GetSummary(request);

            Assert.Equal(expectedSummary, summary);
        }

        [Theory]
        [InlineData(false, false, "{ Match(^FlowValue.*) }")]
        [InlineData(false, true, "{ Func(Name): Match(^FlowValue.*) }")]
        [InlineData(true, false, "{ Func(Match(^FlowValue.*)) }")]
        [InlineData(true, true, "{ Func(Name): Func(Match(^FlowValue.*)) }")]
        public void Input_binding_summary_for_dictionary_regex_name_as_expected(bool isMappedValue, bool isMappedName, string expectedSummary)
        {
            var request = new TestRequest();

            var requestType = request.GetType().GetFlowObjectType();
            var requestProperty = requestType[nameof(TestRequest.InputStrings)];

            var inputBinding = new FlowValueInputBinding(requestProperty)
            {
                FlowValueSelector = 
                    new FlowValueRegexSelector("^FlowValue.*", isMappedName ? n => n : (Func<string, string>)null),
                MapValue = isMappedValue ? v => v : (Func<object, object>)null,
            };

            var summary = inputBinding.GetSummary(request);

            Assert.Equal(expectedSummary, summary);
        }

        [Theory]
        [InlineData(false, false, "{ Is(String) }")]
        [InlineData(false, true, "{ Func(Name): Is(String) }")]
        [InlineData(true, false, "{ Func(Is(String)) }")]
        [InlineData(true, true, "{ Func(Name): Func(Is(String)) }")]
        public void Input_binding_summary_for_dictionary_type_as_expected(bool isMappedValue, bool isMappedName, string expectedSummary)
        {
            var request = new TestRequest();

            var requestType = request.GetType().GetFlowObjectType();
            var requestProperty = requestType[nameof(TestRequest.InputStrings)];

            var inputBinding = new FlowValueInputBinding(requestProperty)
            {
                FlowValueSelector = 
                    new FlowValueTypeSelector(typeof(string), isMappedName ? n => n : (Func<string, string>)null),
                MapValue = isMappedValue ? v => v : (Func<object, object>)null,
            };

            var summary = inputBinding.GetSummary(request);

            Assert.Equal(expectedSummary, summary);
        }

        [Theory]
        [InlineData(false, "FlowInt: OutputInt")]
        [InlineData(true, "FlowInt: Func(OutputInt)")]
        public void Output_binding_summary_for_single_value_as_expected(bool isMappedValue, string expectedSummary)
        {
            var request = new TestRequest();
            var response = new TestResponse();

            var responseType = response.GetType().GetFlowObjectType();
            var responseProperty = responseType[nameof(TestResponse.OutputInt)];

            var outputBinding = new FlowValueOutputBinding(responseProperty)
            {
                MapName = (n, r) => "FlowInt",
                MapValue = isMappedValue ? v => v : (Func<object, object>)null,
            };

            var summary = outputBinding.GetSummary(request);

            Assert.Equal(expectedSummary, summary);
        }

        [Theory]
        [InlineData(false, "{ DictionaryValue1: OutputStrings[DictionaryValue1], DictionaryValue2: OutputStrings[DictionaryValue2] }")]
        [InlineData(true, "{ DictionaryValue1: Func(OutputStrings[DictionaryValue1]), DictionaryValue2: Func(OutputStrings[DictionaryValue2]) }")]
        public void Output_binding_summary_for_dictionary_value_list_as_expected(bool isMappedValue, string expectedSummary)
        {
            var request = new TestRequest();
            var response = new TestResponse();

            var responseType = response.GetType().GetFlowObjectType();
            var responseProperty = responseType[nameof(TestResponse.OutputStrings)];

            var outputBinding = new FlowValueOutputBinding(responseProperty)
            {
                FlowValueSelector = new FlowValueListSelector("DictionaryValue1", "DictionaryValue2"),
                MapValue = isMappedValue ? v => v : (Func<object, object>)null,
            };

            var summary = outputBinding.GetSummary(request);

            Assert.Equal(expectedSummary, summary);
        }

        [Theory]
        [InlineData(false, "{ FlowValue1: OutputStrings[DictionaryValue1], FlowValue2: OutputStrings[DictionaryValue2] }")]
        [InlineData(true, "{ FlowValue1: Func(OutputStrings[DictionaryValue1]), FlowValue2: Func(OutputStrings[DictionaryValue2]) }")]
        public void Output_binding_summary_for_dictionary_value_name_map_as_expected(bool isMappedValue, string expectedSummary)
        {
            var request = new TestRequest();
            var response = new TestResponse();

            var responseType = response.GetType().GetFlowObjectType();
            var responseProperty = responseType[nameof(TestResponse.OutputStrings)];

            var outputBinding = new FlowValueOutputBinding(responseProperty)
            {
                FlowValueSelector = new FlowValueListSelector(new Dictionary<string, string>
                {
                    { "DictionaryValue1", "FlowValue1" },
                    { "DictionaryValue2", "FlowValue2" },
                }),
                MapValue = isMappedValue ? v => v : (Func<object, object>)null,
            };

            var summary = outputBinding.GetSummary(request);

            Assert.Equal(expectedSummary, summary);
        }

        [Theory]
        [InlineData(false, false, "{ Match(^FlowValue.*): OutputStrings[Match(^FlowValue.*)] }")]
        [InlineData(false, true, "{ Func(Name): OutputStrings[Match(^FlowValue.*)] }")]
        [InlineData(true, false, "{ Match(^FlowValue.*): Func(OutputStrings[Match(^FlowValue.*)]) }")]
        [InlineData(true, true, "{ Func(Name): Func(OutputStrings[Match(^FlowValue.*)]) }")]
        public void Output_binding_summary_for_dictionary_regex_name_as_expected(bool isMappedValue, bool isMappedName, string expectedSummary)
        {
            var request = new TestRequest();
            var response = new TestResponse();

            var responseType = response.GetType().GetFlowObjectType();
            var responseProperty = responseType[nameof(TestResponse.OutputStrings)];

            var outputBinding = new FlowValueOutputBinding(responseProperty)
            {
                FlowValueSelector =
                    new FlowValueRegexSelector("^FlowValue.*", isMappedName ? n => n : (Func<string, string>)null),
                MapValue = isMappedValue ? v => v : (Func<object, object>)null,
            };

            var summary = outputBinding.GetSummary(request);

            Assert.Equal(expectedSummary, summary);
        }

        [Theory]
        [InlineData(false, false, "{ Is(String): OutputStrings[Is(String)] }")]
        [InlineData(false, true, "{ Func(Name): OutputStrings[Is(String)] }")]
        [InlineData(true, false, "{ Is(String): Func(OutputStrings[Is(String)]) }")]
        [InlineData(true, true, "{ Func(Name): Func(OutputStrings[Is(String)]) }")]
        public void Output_binding_summary_for_dictionary_type_as_expected(bool isMappedValue, bool isMappedName, string expectedSummary)
        {
            var request = new TestRequest();
            var response = new TestResponse();

            var responseType = response.GetType().GetFlowObjectType();
            var responseProperty = responseType[nameof(TestResponse.OutputStrings)];

            var outputBinding = new FlowValueOutputBinding(responseProperty)
            {
                FlowValueSelector = 
                    new FlowValueTypeSelector(typeof(string), isMappedName ? n => n : (Func<string, string>)null),
                MapValue = isMappedValue ? v => v : (Func<object, object>)null,
            };

            var summary = outputBinding.GetSummary(request);

            Assert.Equal(expectedSummary, summary);
        }

        #endregion

        #region Private methods

        private void AssertDiagramFlow(FlowDiagram flowDiagram, IEnumerable<string> expectedFlowLines)
        {
            var dotNotation = flowDiagram.GetDotNotation();
            Assert.NotEmpty(dotNotation);

            _output.WriteLine(dotNotation);

            var actualLines = Regex.Split(dotNotation, "\r\n|\r|\n");

            foreach (var expectedLine in expectedFlowLines)
            {
                Assert.True(actualLines.Any(al => al.Contains(expectedLine)),
                    $"Actual diagram did not contain: {expectedLine}");
            }
        }

        private IMediator GetMediator(IFlowOverrideProvider overrideProvider = null)
        {
            var serviceCollection = new ServiceCollection()
                .AddMediatR(typeof(IFlowHandler).Assembly);

            FlowDiscovery.RegisterFlowTypes(typeof(EmptyFlow).Assembly,
                (intType, impType) => serviceCollection.AddSingleton(intType, impType));

            if (overrideProvider != null)
            {
                serviceCollection.AddSingleton(overrideProvider);
            }

            serviceCollection.BuildServiceProvider(this, out var mediator, out _);
            return mediator;
        }

        #endregion
    }
}
