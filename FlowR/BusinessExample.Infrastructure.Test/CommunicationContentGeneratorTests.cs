using System;
using System.Collections.Generic;
using System.Text;
using BusinessExample.Core.Entities;
using Xunit;

namespace BusinessExample.Infrastructure.Test
{
    public class CommunicationContentGeneratorTests
    {
        [Theory]
        [InlineData("AcceptConfirmation", "accepted")]
        [InlineData("ReferNotification", "referred")]
        [InlineData("DeclineConfirmation", "declined")]
        public async void Can_generate_confirmations(string templateName, string expectedWording)
        {
            var loanApplication = new LoanApplication
            {
                ApplicantName = "Bertie Bassett",
                LoanAmount = 10000.66m,
                Reference = "A1000006524",
            };

            var sut = new CommunicationContentGenerator();

            var content =
                await sut.GenerateContent(templateName, new Dictionary<string, object>
                {
                    { "LoanApplication", loanApplication }
                });

            Assert.Matches(expectedWording, content);
            Assert.Matches("Bertie Bassett", content);
            Assert.Matches("10000.66", content);
            Assert.Matches("A1000006524", content);
        }
    }
}
