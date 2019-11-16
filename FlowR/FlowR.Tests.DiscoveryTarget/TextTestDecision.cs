using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace FlowR.Tests.DiscoveryTarget
{
    public class TextTestDecisionRequest : NullableFlowValueDecision<string>
    {
        public string SetValue { get; set; }

        public override string GetText()
        {
            return $"SetValue={this.SetValue}";
        }
    }
}
