using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR
{
    public interface IFlowDecisionRequestHandler<TReq, TSwitch> 
        : IRequestHandler<TReq, int> where TReq : FlowDecisionRequest<TSwitch>
    {
    }
}
