using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace FlowR.Tests.DiscoveryTarget
{
    public class ConfigurableDecisionRequest : NullableFlowValueDecision<string>
    {
        [DesignTimeValue]
        public string SetValue { get; set; }

        public string BoundValue { get; set; }
    }
}
