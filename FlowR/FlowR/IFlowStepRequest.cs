using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR
{
    public interface IFlowStepRequest
    {
        FlowContext FlowContext { get; set; }

        string GetText();
    }
}
