using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR.Tests.DiscoveryTarget
{
    [Description("Basic decision description")]
    public class BasicDecisionRequest : NullableFlowValueDecision<string>
    {
    }

    public class BasicDecision : FlowValueDecisionHandler<BasicDecisionRequest, string>
    {
    }
}
