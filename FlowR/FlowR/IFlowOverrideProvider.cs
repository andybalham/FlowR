using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR
{
    public interface IFlowOverrideProvider
    {
        IEnumerable<FlowRequestOverride> GetRequestOverrides(string overrideKey);

        IDictionary<string, FlowRequestOverride> GetApplicableRequestOverrides(IList<FlowRequestOverride> overrides, IFlowStepRequest request);

        IEnumerable<FlowDefinition> GetFlowDefinitionOverrides(Type requestType);

        FlowDefinition GetApplicableFlowDefinitionOverride(IList<FlowDefinition> overrides, IFlowStepRequest request);
    }
}
