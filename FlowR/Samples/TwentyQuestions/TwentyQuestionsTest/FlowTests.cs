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
        [Theory]
        [InlineData("Y", "4", "Y", "", "", "Horse")]
        [InlineData("Y", "4", "N", "", "", "Cat")]
        [InlineData("Y", "2", "", "N", "", "Farmer")]
        [InlineData("Y", "2", "", "Y", "", "Duck")]
        [InlineData("N", "", "", "", "Y", "Snake")]
        [InlineData("N", "", "", "", "N", "Worm")]
        public void RunFlow(string hasLegs, string legCount, string eatsHay, string canFly, string hasScales, string expectedGuess)
        {
            var serviceCollection =
                new ServiceCollection()
                    .AddMediatR(typeof(TwentyQuestionsRequest).Assembly);

            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                var flowContext = new FlowContext()
                    .MockQuestionActivity(new Dictionary<string, string>
                    {
                        { "AskHasLegs", hasLegs },
                        { "AskLegCount", legCount },
                        { "AskEatsHay", eatsHay },
                        { "AskCanFly", canFly },
                        { "AskHasScales", hasScales },
                    })
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

