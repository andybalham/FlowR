using FlowR;
using MediatR;

namespace BusinessExample.Core.Exchanges
{
    public abstract class BusinessFlowHandler<TFlowRequest, TFlowResponse> : FlowHandler<TFlowRequest, TFlowResponse>
        where TFlowRequest : FlowActivityRequest<TFlowResponse>
        where TFlowResponse : FlowResponse
    {
        protected BusinessFlowHandler(IMediator mediator, IFlowLoggerBase logger) : base(mediator, logger)
        {
        }
    }
}
