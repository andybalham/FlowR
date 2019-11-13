using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace FlowR.StepLibrary.Activities
{
    public abstract class SetFlowValueRequestBase<TRes, TVal> : FlowActivityRequest<TRes> where TRes : SetFlowValueResponse<TVal>
    {
        public TVal OutputValue { get; set; }
    }

    public class SetFlowValueResponse<TVal>
    {
        public TVal Output { get; set; }
    }

    public class SetFlowValueRequest : SetFlowValueRequestBase<SetFlowValueResponse, object> { }

    public class SetFlowValueResponse : SetFlowValueResponse<object> { }

    public class SetFlowValueActivity : IRequestHandler<SetFlowValueRequest, SetFlowValueResponse>
    {
        public Task<SetFlowValueResponse> Handle(SetFlowValueRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SetFlowValueResponse { Output = request.OutputValue });
        }
    }

    public class SetStringFlowValueRequest : SetFlowValueRequestBase<SetStringFlowValueResponse, string> { }

    public class SetStringFlowValueResponse : SetFlowValueResponse<string> { }

    public class SetStringFlowValueActivity : IRequestHandler<SetStringFlowValueRequest, SetStringFlowValueResponse>
    {
        public Task<SetStringFlowValueResponse> Handle(SetStringFlowValueRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SetStringFlowValueResponse { Output = request.OutputValue });
        }
    }

    public class SetBoolFlowValueRequest : SetFlowValueRequestBase<SetBoolFlowValueResponse, bool> { }

    public class SetBoolFlowValueResponse : SetFlowValueResponse<bool> { }

    public class SetBoolFlowValueActivity : IRequestHandler<SetBoolFlowValueRequest, SetBoolFlowValueResponse>
    {
        public Task<SetBoolFlowValueResponse> Handle(SetBoolFlowValueRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SetBoolFlowValueResponse { Output = request.OutputValue });
        }
    }
}
