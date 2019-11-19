using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR
{
    public abstract class FlowActivityRequest<T> : IFlowStepRequest, IRequest<T>
    {
        public FlowContext FlowContext { get; set; }

        public virtual string GetText() => null;
    }
}
