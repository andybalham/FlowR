using System;
using System.Collections.Generic;
using FlowR;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TwentyQuestions.Core;
using Xunit;

namespace TwentyQuestions.Test
{
    public class FlowTests
    {
        [Fact]
        public async void TwentyQuestionsRequest_is_valid_flow()
        {
            var serviceCollection =
                new ServiceCollection()
                    .AddMediatR(typeof(FlowValidationRequest).Assembly)
                    .AddMediatR(typeof(TwentyQuestionsHandler).Assembly);

            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                var mediator = serviceProvider.GetService<IMediator>();

                var response = await mediator.Send(new FlowValidationRequest());

                Assert.Empty(response.Errors);
            }
        }

        [Theory]
        [InlineData("Y", "4", "Y", "", "", "Horse")]
        [InlineData("Y", "4", "N", "", "", "Cat")]
        [InlineData("Y", "2", "", "N", "", "Farmer")]
        [InlineData("Y", "2", "", "Y", "", "Duck")]
        [InlineData("N", "", "", "", "Y", "Snake")]
        [InlineData("N", "", "", "", "N", "Worm")]
        public void TwentyQuestions_returns_expected_guess(
            string hasLegsAnswer, string legCountAnswer, string eatsHayAnswer, string canFlyAnswer, string hasScalesAnswer, 
            string expectedGuess)
        {
            var serviceCollection =
                new ServiceCollection()
                    .AddMediatR(typeof(TwentyQuestionsRequest).Assembly);

            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                var flowContext = new FlowContext()
                    .MockQuestionActivity("AskHasLegs", hasLegsAnswer)
                    .MockQuestionActivity("AskLegCount", legCountAnswer)
                    .MockQuestionActivity("AskEatsHay", eatsHayAnswer)
                    .MockQuestionActivity("AskCanFly", canFlyAnswer)
                    .MockQuestionActivity("AskHasScales", hasScalesAnswer)
                    .MockGuessActivity();

                var mediator = serviceProvider.GetService<IMediator>();

                var response =
                    mediator.Send(new TwentyQuestionsRequest { FlowContext = flowContext })
                        .GetAwaiter().GetResult();

                Assert.Equal(expectedGuess, response.Guess);
            }
        }
    }
}

