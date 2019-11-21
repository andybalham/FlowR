using FlowR;
using MediatR;

namespace TwentyQuestions
{
    public class TwentyQuestionsRequest : FlowActivityRequest<TwentyQuestionsResponse>
    {
        
    }

    public class TwentyQuestionsResponse : FlowResponse
    {
        public string Guess { get; set; }
    }

    public class TwentyQuestionsHandler : FlowHandler<TwentyQuestionsRequest, TwentyQuestionsResponse>
    {
        public TwentyQuestionsHandler(IMediator mediator, IFlowLogger<TwentyQuestionsHandler> logger) : base(mediator, logger)
        {
        }

        protected override void OnDebugEvent(string stepName, FlowDebugEvent debugEvent, FlowValues flowValues)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()

                .Do("AskHasLegs", QuestionRequest.NewDefinition()
                    .SetValue(req => req.Question, "Does it have legs")
                    .SetValue(req => req.Answers, new[] { "[Y]es", "[N]o" })
                    .BindOutput(res => res.Answer, "HasLegs"))

                .Check("HasLegs", "Has legs?", FlowValueDecision<string>.NewDefinition()
                    .BindInput(req => req.SwitchValue, "HasLegs"))
                .When("Y").Goto("AskLegCount")
                .When("N").Goto("AskHasScales")
                .Else().Exception()

                .Do("AskLegCount", QuestionRequest.NewDefinition()
                    .SetValue(req => req.Question, "How many legs does it have")
                    .SetValue(req => req.Answers, new[] { "[2]", "[4]" })
                    .BindOutput(res => res.Answer, "LegCount"))

                .Check("LegCount", "Leg count?", FlowValueDecision<string>.NewDefinition()
                    .BindInput(req => req.SwitchValue, "LegCount"))
                .When("2").Goto("AskCanFly")
                .When("4").Goto("AskEatsHay")
                .Else().Exception()

                .Do("AskCanFly", QuestionRequest.NewDefinition()
                    .SetValue(req => req.Question, "Can it fly")
                    .SetValue(req => req.Answers, new[] { "[Y]es", "[N]o" })
                    .BindOutput(res => res.Answer, "CanFly"))

                .Check("CanFly", "Can fly?", FlowValueDecision<string>.NewDefinition()
                    .BindInput(req => req.SwitchValue, "CanFly"))
                .When("Y").Goto("GuessDuck")
                .When("N").Goto("GuessFarmer")
                .Else().Exception()

                .Do("AskEatsHay", QuestionRequest.NewDefinition()
                    .SetValue(req => req.Question, "Does it eat hay")
                    .SetValue(req => req.Answers, new[] { "[Y]es", "[N]o" })
                    .BindOutput(res => res.Answer, "EatsHay"))

                .Check("EatsHay", "Eats hay?", FlowValueDecision<string>.NewDefinition()
                    .BindInput(req => req.SwitchValue, "EatsHay"))
                .When("Y").Goto("GuessHorse")
                .When("N").Goto("GuessCat")
                .Else().Exception()

                .Do("AskHasScales", QuestionRequest.NewDefinition()
                    .SetValue(req => req.Question, "Does it have scales")
                    .SetValue(req => req.Answers, new[] { "[Y]es", "[N]o" })
                    .BindOutput(res => res.Answer, "HasScales"))

                .Check("HasScales", "Has scales?", FlowValueDecision<string>.NewDefinition()
                    .BindInput(req => req.SwitchValue, "HasScales"))
                .When("Y").Goto("GuessSnake")
                .When("N").Goto("GuessWorm")
                .Else().Exception()

                .Do("GuessDuck", GuessRequest.NewDefinition()
                    .SetValue(req => req.Guess, "Duck"))
                .End()

                .Do("GuessFarmer", GuessRequest.NewDefinition()
                    .SetValue(req => req.Guess, "Farmer"))
                .End()

                .Do("GuessHorse", GuessRequest.NewDefinition()
                    .SetValue(req => req.Guess, "Horse"))
                .End()

                .Do("GuessCat", GuessRequest.NewDefinition()
                    .SetValue(req => req.Guess, "Cat"))
                .End()

                .Do("GuessSnake", GuessRequest.NewDefinition()
                    .SetValue(req => req.Guess, "Snake"))
                .End()

                .Do("GuessWorm", GuessRequest.NewDefinition()
                    .SetValue(req => req.Guess, "Worm"))
                .End();
        }
    }
}