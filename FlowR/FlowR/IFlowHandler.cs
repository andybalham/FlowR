using FlowR.Discovery;
using System;

namespace FlowR
{
    public interface IFlowHandler
    {
        Type RequestType { get; }

        Type ResponseType { get; }

        FlowDefinition GetFlowDefinition();
    }
}
