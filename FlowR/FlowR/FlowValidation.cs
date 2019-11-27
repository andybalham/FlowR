using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MediatR;

namespace FlowR
{
    public static class FlowValidation
    {
        public static void RegisterFlowTypes(Assembly assembly, Action<Type, Type> registerFlowType)
        {
            var flowTypes = assembly.GetExportedTypes().Where(t => typeof(IFlowHandler).IsAssignableFrom(t));

            foreach (var flowType in flowTypes)
            {
                registerFlowType(typeof(IFlowHandler), flowType);
            }
        }
    }

    public class FlowValidationRequest : IRequest<FlowValidationResponse>
    {
    }

    public class FlowValidationResponse
    {
        public Dictionary<string, string[]> Errors { get; set; }
    }

    public class FlowValidationHandler : RequestHandler<FlowValidationRequest, FlowValidationResponse>
    {
        private readonly IEnumerable<IFlowHandler> _flows;
        private readonly IFlowOverrideProvider _overrideProvider;

        public FlowValidationHandler(IEnumerable<IFlowHandler> flows, IFlowOverrideProvider overrideProvider = null)
        {
            _flows = flows;
            _overrideProvider = overrideProvider;
        }

        protected override FlowValidationResponse Handle(FlowValidationRequest request)
        {
            var errors = new Dictionary<string, string[]>();

            foreach (var flow in _flows)
            {
                var baseDefinition = flow.GetFlowDefinition();
                var baseErrors = baseDefinition.Validate().ToArray();

                if (baseErrors.Length > 0)
                {
                    errors.Add($"{flow.RequestType.FullName}", baseErrors);
                }

                var flowDefinitionOverrides =
                    _overrideProvider?.GetFlowDefinitionOverrides(flow.RequestType)?.ToList();

                if (flowDefinitionOverrides != null)
                {
                    foreach (var flowDefinitionOverride in flowDefinitionOverrides)
                    {
                        var overrideErrors = flowDefinitionOverride.Validate().ToArray();

                        if (overrideErrors.Length > 0)
                        {
                            errors.Add($"{flow.RequestType.FullName}[{flowDefinitionOverride.Criteria}]", overrideErrors);
                        }
                    }
                }
            }

            return new FlowValidationResponse { Errors = errors };
        }
    }
}
