using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Moq;
using TwentyQuestions.Core;
using Xunit;

namespace TwentyQuestions.Test
{
    public class ActivityTests
    {
        [Theory]
        [InlineData(new[] { "y" }, "Y")]
        [InlineData(new[] { "Y" }, "Y")]
        [InlineData(new[] { "N" }, "N")]
        [InlineData(new[] { "X", "Y" }, "Y")]
        [InlineData(new[] { "X", "X", "Y" }, "Y")]
        public void Fact_description(string[] readLineResponses, string expectedAnswer)
        {
            // Arrange

            var request = new QuestionRequest
            {
                Answers = new[] { "[Y]es", "[N]o" }
            };

            var consoleServiceMock = new Mock<IConsoleService>();

            foreach (var readLineResponse in readLineResponses)
            {
                consoleServiceMock.Setup(mock => mock.ReadLine()).Returns(readLineResponse);
            }

            var sut = new QuestionHandler(consoleServiceMock.Object);

            // Act

            var response = sut.Handle(request, CancellationToken.None).GetAwaiter().GetResult();

            // Assert

            Assert.Equal(expectedAnswer, response.Answer);
        }
    }
}
