using System;
using FlowR.Tests.Domain.OverrideProviderTests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlowR.StepLibrary.Activities;
using Xunit;
using Xunit.Abstractions;

namespace FlowR.Tests
{
    public class OverrideProviderTests : TestBase
    {
        #region Constructors

        public OverrideProviderTests(ITestOutputHelper output) : base(output)
        {
        }

        #endregion

        #region Facts and theories

        [Fact]
        public async void Can_override_request_property()
        {
            var (mediator, _) = 
                GetMediator<OverriddenActivityFlowRequest>(sc => 
                    sc.AddTransient(typeof(IFlowOverrideProvider), typeof(BasicStepOverrideProvider)));

            var request = new OverriddenActivityFlowRequest();

            var response = await mediator.Send(request);

            Assert.Equal(BasicStepOverrideProvider.OverriddenValue, response.OverridableOutputValue);
            Assert.Equal(OverriddenActivityFlowRequest.BaseValue, response.NonOverridableOutputValue);
        }

        [Theory]
        [InlineData("Client1AppSettings.json", "FlowValue1", "Client1-FlowValue1-Override")]
        [InlineData("Client1AppSettings.json", "FlowValue2", "Client1-Override")]
        [InlineData("Client1AppSettings.json", "FlowValue3", "Client1-Override")]
        [InlineData("Client2AppSettings.json", "FlowValue1", "Base-Override")]
        [InlineData("Client2AppSettings.json", "FlowValue2", "FlowValue2-Override")]
        public async void Can_override_activity_request_property_with_criteria(
            string settingsFileName, string flowValue, string expectedOverriddenValue)
        {
            var configurationRoot =
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(settingsFileName)
                    .Build();

            var (mediator, _) =
                GetMediator<OverriddenActivityFlowRequest>(sc =>
                {
                    sc.AddTransient(typeof(IFlowOverrideProvider), typeof(CriteriaBasedStepOverrideProvider));
                    sc.AddSingleton<IConfiguration>(configurationRoot);
                });

            var request = new OverriddenActivityFlowRequest { FlowValue = flowValue };

            var response = await mediator.Send(request);

            Assert.Equal(expectedOverriddenValue, response.OverridableOutputValue);
        }

        [Fact]
        public async void Can_override_flow_definition()
        {
            var (mediator, _) =
                GetMediator<OverriddenFlowRequest>(sc =>
                    sc.AddTransient(typeof(IFlowOverrideProvider), typeof(BasicFlowOverrideProvider)));

            var request = new OverriddenFlowRequest();

            var response = await mediator.Send(request);

            Assert.NotEqual(OverriddenFlowRequest.BaseValue, response.OutputValue);
            Assert.Equal(BasicFlowOverrideProvider.OverriddenValue, response.OutputValue);
        }

        [Theory]
        [InlineData("FlowValue1", CriteriaBasedFlowOverrideProvider.OverriddenValue)]
        [InlineData("FlowValue2", OverriddenFlowRequest.BaseValue)]
        public async void Can_override_flow_definition_with_criteria(string flowValue, string expectedOutputValue)
        {
            var (mediator, _) =
                GetMediator<OverriddenFlowRequest>(sc =>
                    sc.AddTransient(typeof(IFlowOverrideProvider), typeof(CriteriaBasedFlowOverrideProvider)));

            var request = new OverriddenFlowRequest { FlowValue = flowValue };

            var response = await mediator.Send(request);

            Assert.Equal(expectedOutputValue, response.OutputValue);
        }

        #endregion

        #region Override providers

        private class BasicStepOverrideProvider : TestOverrideProviderBase
        {
            public const string OverriddenValue = "OverriddenValue";

            private readonly Dictionary<string, FlowRequestOverride[]> _flowRequestOverridesDictionary =
                new Dictionary<string, FlowRequestOverride[]>()
                {
                    {
                        "ActivityOverrideKey", new[]
                        {
                            new FlowRequestOverride()
                            {
                                Name = nameof(OverridableActivityRequest.OverridableInputValue),
                                Value = OverriddenValue
                            },
                            new FlowRequestOverride()
                            {
                                Name = nameof(OverridableActivityRequest.NonOverridableInputValue),
                                Value = OverriddenValue
                            },
                        }
                    }
                };

            public override IEnumerable<FlowRequestOverride> GetRequestOverrides(string overrideKey)
            {
                return _flowRequestOverridesDictionary.TryGetValue(overrideKey, out var flowRequestOverrides) 
                    ? flowRequestOverrides : null;
            }

            public override IDictionary<string, FlowRequestOverride> GetApplicableRequestOverrides(
                IList<FlowRequestOverride> overrides, IFlowStepRequest request)
            {
                return overrides.ToDictionary(@override => @override.Name);
            }
        }

        private class CriteriaBasedStepOverrideProvider : TestOverrideProviderBase
        {
            private readonly ClientSettings _clientSettings;

            public CriteriaBasedStepOverrideProvider(IConfiguration configuration)
            {
                _clientSettings = configuration.GetSection("ClientSettings").Get<ClientSettings>();
            }

            private readonly Dictionary<string, FlowRequestOverride[]> _flowRequestOverridesDictionary =
                new Dictionary<string, FlowRequestOverride[]>()
                {
                    {
                        "ActivityOverrideKey", new[]
                        {
                            new FlowRequestOverride()
                            {
                                Name = nameof(OverridableActivityRequest.OverridableInputValue),
                                Value = "Client1-FlowValue1-Override",
                                Criteria = "ClientId=Client1;FlowValue=FlowValue1"
                            },
                            new FlowRequestOverride()
                            {
                                Name = nameof(OverridableActivityRequest.OverridableInputValue),
                                Value = "Client1-Override",
                                Criteria = "ClientId=Client1"
                            },
                            new FlowRequestOverride()
                            {
                                Name = nameof(OverridableActivityRequest.OverridableInputValue),
                                Value = "FlowValue2-Override",
                                Criteria = "FlowValue=FlowValue2"
                            },
                            new FlowRequestOverride()
                            {
                                Name = nameof(OverridableActivityRequest.OverridableInputValue),
                                Value = "Base-Override",
                            },
                            new FlowRequestOverride()
                            {
                                Name = "NonExistentProperty",
                                Value = "Base-Override",
                            },
                        }
                    }
                };

            public override IEnumerable<FlowRequestOverride> GetRequestOverrides(string overrideKey)
            {
                return _flowRequestOverridesDictionary.TryGetValue(overrideKey, out var flowRequestOverrides)
                    ? flowRequestOverrides : null;
            }

            public override IDictionary<string, FlowRequestOverride> GetApplicableRequestOverrides(
                IList<FlowRequestOverride> overrides, IFlowStepRequest request)
            {
                var clientId = _clientSettings.ClientId;
                var flowValue = (request is ITestOverrideContext context) ? context.FlowValue : null;

                var applicableOverrides = 
                    overrides.GroupBy(o => o.Name)
                        .Select(og => GetApplicableRequestOverride(og.ToList(), clientId, flowValue));

                return applicableOverrides.ToDictionary(o => o.Name);
            }

            private static FlowRequestOverride GetApplicableRequestOverride(
                ICollection<FlowRequestOverride> propertyOverrides, string clientId, string flowValue)
            {
                switch (propertyOverrides.Count)
                {
                    case 0:
                        return null;

                    case 1 when string.IsNullOrEmpty(propertyOverrides.First().Criteria):
                        return propertyOverrides.First();
                }

                var criteriaCandidates = new[]
                {
                    $"ClientId={clientId};FlowValue={flowValue}",
                    $"ClientId={clientId}",
                    $"FlowValue={flowValue}",
                    null,
                };

                var propertyOverride = (FlowRequestOverride)null;

                foreach (var criteriaCandidate in criteriaCandidates)
                {
                    var matchingOverrides = 
                        propertyOverrides.Where(o => o.Criteria == criteriaCandidate).ToList();

                    if (matchingOverrides.Count == 1)
                    {
                        propertyOverride = matchingOverrides.First();
                        break;
                    }

                    if (matchingOverrides.Count > 1)
                    {
                        throw new FlowException(
                            $"The request criteria '{criteriaCandidate}' matched {matchingOverrides.Count} overrides " +
                            "when at most 1 was expected");
                    }
                }

                return propertyOverride;
            }
        }

        private class BasicFlowOverrideProvider : TestOverrideProviderBase
        {
            public const string OverriddenValue = "OverriddenValue";

            public override IEnumerable<IFlowDefinition> GetFlowDefinitionOverrides(Type requestType)
            {
                if (requestType == typeof(OverriddenFlowRequest))
                {
                    return new[]
                    {
                        new FlowDefinition<OverriddenFlowRequest, OverriddenFlowResponse>()
                            .Do("Activity",
                                new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                                    .SetValue(rq => rq.OutputValue, OverriddenValue)
                                    .BindOutput(rq => rq.Output, nameof(OverriddenFlowResponse.OutputValue)))
                    };
                }

                return null;
            }

            public override IFlowDefinition GetApplicableFlowDefinitionOverride(IList<IFlowDefinition> overrides, IFlowStepRequest request)
            {
                return overrides?.First();
            }
        }

        private class CriteriaBasedFlowOverrideProvider : TestOverrideProviderBase
        {
            public const string OverriddenValue = "OverriddenValue";

            public override IEnumerable<IFlowDefinition> GetFlowDefinitionOverrides(Type requestType)
            {
                if (requestType == typeof(OverriddenFlowRequest))
                {
                    return new[]
                    {
                        new FlowDefinition<OverriddenFlowRequest, OverriddenFlowResponse>("FlowValue=FlowValue1")
                            .Do("Activity",
                                new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                                    .SetValue(rq => rq.OutputValue, OverriddenValue)
                                    .BindOutput(rq => rq.Output, nameof(OverriddenFlowResponse.OutputValue)))
                    };
                }

                return null;
            }

            public override IFlowDefinition GetApplicableFlowDefinitionOverride(IList<IFlowDefinition> overrides, IFlowStepRequest request)
            {
                switch (overrides?.Count)
                {
                    case 0:
                        return null;

                    case 1 when string.IsNullOrEmpty(overrides.First().Criteria):
                        return overrides.First();
                }

                var flowValue = (request is ITestOverrideContext context) ? context.FlowValue : null;

                var matchingOverride = overrides?.Where(o => o.Criteria == $"FlowValue={flowValue}").FirstOrDefault();

                return matchingOverride ?? overrides?.Where(o => string.IsNullOrEmpty(o.Criteria)).FirstOrDefault();
            }
        }

        #endregion
    }

    internal class ClientSettings
    {
        public string ClientId { get; set; }
    }
}
