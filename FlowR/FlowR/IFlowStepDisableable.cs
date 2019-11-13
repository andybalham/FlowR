using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR
{
    public interface IFlowStepDisableable
    {
        bool IsDisabled { get; }
    }
}
