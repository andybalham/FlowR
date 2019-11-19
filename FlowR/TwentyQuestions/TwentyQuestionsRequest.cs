using FlowR;
using MediatR;

namespace TwentyQuestions
{
    public class TwentyQuestionsRequest : FlowActivityRequest<TwentyQuestionsResponse>
    {
        
    }

    public class TwentyQuestionsResponse
    {
    }

    public class TwentyQuestionsHandler : FlowHandler<TwentyQuestionsRequest, TwentyQuestionsResponse>
    {
        public TwentyQuestionsHandler(IMediator mediator, IFlowLogger<TwentyQuestionsHandler> logger) : base(mediator, logger)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition();
        }
    }
}