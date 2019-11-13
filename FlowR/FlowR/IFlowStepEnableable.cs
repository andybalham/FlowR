using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR
{
    public interface IFlowStepEnableable
    {
        bool IsEnabled { get; }
    }
}
