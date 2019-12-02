using System;
using System.Collections.Generic;

namespace FlowR.Tests
{
    internal class TestOverrideProviderBase : IFlowOverrideProvider
    {
        public virtual IEnumerable<FlowRequestOverride> GetRequestOverrides(string overrideKey)
        {
            return null;
        }

        public virtual IDictionary<string, FlowRequestOverride> GetApplicableRequestOverrides(IList<FlowRequestOverride> overrides, IFlowStepRequest request)
        {
            return null;
        }

        public virtual IEnumerable<IFlowDefinition> GetFlowDefinitionOverrides(Type requestType)
        {
            return null;
        }

        public virtual IFlowDefinition GetApplicableFlowDefinitionOverride(IList<IFlowDefinition> overrides, IFlowStepRequest request)
        {
            return null;
        }
    }
}