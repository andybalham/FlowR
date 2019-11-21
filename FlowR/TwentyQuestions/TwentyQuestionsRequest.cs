using FlowR;
using MediatR;

namespace TwentyQuestions
{
    public class TwentyQuestionsRequest : FlowActivityRequest<TwentyQuestionsResponse>
    {
        
    }

    public class TwentyQuestionsResponse : FlowResponse
    {
        public string Summary { get; set; }
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

                .Do("AskQuestionsOrDiagram", QuestionRequest.NewDefinition()
                    .SetValue(req => req.Question, "Questions or diagram")
                    .SetValue(req => req.Answers, new[] { "[Q]uestions", "[D]iagram" })
                    .BindOutput(res => res.Answer, "QuestionsOrDiagram"))

                .Check("QuestionsOrDiagram", "Questions or diagram?", FlowValueDecision<string>.NewDefinition()
                    .BindInput(req => req.SwitchValue, "QuestionsOrDiagram"))
                .When("Q").Goto("Questions")
                .When("D").Goto("OutputDiagram")
                .Else().Exception()

                .Do("OutputDiagram", DotNotationRequest.NewDefinition()
                    .SetValue(req => req.TargetRequestType, typeof(TwentyQuestionsRequest)))
                .End()

                .Label("Questions")

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
                .Goto("BuildSummary")

                .Do("GuessFarmer", GuessRequest.NewDefinition()
                    .SetValue(req => req.Guess, "Farmer"))
                .Goto("BuildSummary")

                .Do("GuessHorse", GuessRequest.NewDefinition()
                    .SetValue(req => req.Guess, "Horse"))
                .Goto("BuildSummary")

                .Do("GuessCat", GuessRequest.NewDefinition()
                    .SetValue(req => req.Guess, "Cat"))
                .Goto("BuildSummary")

                .Do("GuessSnake", GuessRequest.NewDefinition()
                    .SetValue(req => req.Guess, "Snake"))
                .Goto("BuildSummary")

                .Do("GuessWorm", GuessRequest.NewDefinition()
                    .SetValue(req => req.Guess, "Worm"))
                .Goto("BuildSummary")

                .Do("BuildSummary", BuildSummaryRequest.NewDefinition()
                    .BindInputs(req => req.Values, "HasLegs", "LegCount", "CanFly", "HasScales", "EatsHay", "Guess"))
                .End();
        }
    }
}